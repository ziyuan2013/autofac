using NUnit.Framework;
using System;
using Autofac.Activators;
using System.Collections.Generic;
using Autofac.Registry;
using Autofac.Lifetime;
using Autofac.Services;
using Autofac.Disposal;

namespace Autofac.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void ResolveOptionalReturnsInstanceWhenRegistered()
        {
            var target = new Container();
            target.RegisterInstance("Hello");

            var inst = target.ResolveOptional<string>();

            Assert.AreEqual("Hello", inst);
        }

        [Test]
        public void ResolveOptionalReturnsNullWhenNotRegistered()
        {
            var target = new Container();
            var inst = target.ResolveOptional<string>();
            Assert.IsNull(inst);
        }

        [Test]
        public void CanRegisterAndResolveSameInstance()
        {
            var instance = new object();
            var container = new Container();
            container.RegisterInstance(instance);
            var resolved = container.Resolve<object>();
            Assert.AreSame(instance, resolved);
            Assert.IsTrue(container.IsRegistered<object>());
        }

        [Test]
        public void LastRegistrationForAServiceBecomesTheDefault()
        {
            var target = new Container();

            var instance1 = new object();
            var instance2 = new object();

            target.RegisterInstance(instance1);
            target.RegisterInstance(instance2);

            Assert.AreSame(instance2, target.Resolve<object>());
        }

        [Test]
        public void CanRegisterComponentDirectly()
        {
            var registration = CreateRegistration(
                new ProvidedInstanceActivator("Hello"),
                new[] { new TypedService(typeof(object)), new TypedService(typeof(string)) });

            var target = new Container();

            target.RegisterComponent(registration);

            Assert.IsTrue(target.IsRegistered<object>());
            Assert.IsTrue(target.IsRegistered<string>());
        }

        private static ComponentRegistration CreateRegistration(IInstanceActivator activator, IEnumerable<Service> services)
        {
            var registration = new ComponentRegistration(
                Guid.NewGuid(),
                activator,
                new RootScopeLifetime(),
                InstanceSharing.Shared,
                InstanceOwnership.OwnedByLifetimeScope,
                services,
                new Dictionary<string, object>());
            return registration;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CannotRegisterNullAsAComponent()
        {
            var target = new Container();

            target.RegisterComponent(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void CannotRegisterNullAsAService()
        {
            var registration = CreateRegistration(
                new ProvidedInstanceActivator(new object()),
                new Service[] { new TypedService(typeof(object)), null });
        }

        [Test]
        public void CanRegisterType()
        {
            var target = new Container();
            target.RegisterType<object>();
            var instance = target.Resolve<object>();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(typeof(object), instance);
        }

        [Test]
        public void CanRegisterTypeNonGenerically()
        {
            var target = new Container();
            target.RegisterType(typeof(object));
            var instance = target.Resolve<object>();
            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(typeof(object), instance);
        }

        [Test]
        public void CanRegisterDelegate()
        {
            object instance = new object();
            var target = new Container();
            target.RegisterDelegate((c, p) => instance);
            Assert.AreSame(instance, target.Resolve<object>());
        }

        [Test]
        public void ResolveUnregistered()
        {
            try
            {
                var target = new Container();
                target.Resolve<object>();
            }
            catch (ComponentNotRegisteredException se)
            {
                Assert.IsTrue(se.Message.Contains("System.Object"));
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected a ComponentNotRegisteredException, got {0}.", ex);
                return;
            }

            Assert.Fail("Expected a ComponentNotRegisteredException.");
        }

        [Test]
        public void UnsharedComponentsProvideNewInstanceOnEveryResolve()
        {
            var container = new Container();
            container.RegisterType<object>().UnsharedInstances();
        }

        [Test]
        public void CircularDependencyIsDetected()
        {
            var target = new Container();
            target.RegisterDelegate(c => c.Resolve<object>());
            try
            {
                target.Resolve<object>();
            }
            catch (DependencyResolutionException de)
            {
                Assert.IsNull(de.InnerException);
                Assert.IsTrue(de.Message.Contains("System.Object -> System.Object"));
                Assert.IsFalse(de.Message.Contains("System.Object -> System.Object -> System.Object"));
                return;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected a DependencyResolutionException, got {0}.", ex);
                return;
            }

            Assert.Fail("Expected a DependencyResolutionException.");
        }

        // In the below scenario, B depends on A, CD depends on A and B,
        // and E depends on IC and B.

        #region Scenario Classes

        class A : DisposeTracker { }

        class B : DisposeTracker
        {
            public A A;

            public B(A a)
            {
                A = a;
            }
        }

        interface IC { }

        class C : DisposeTracker
        {
            public B B;

            public C(B b)
            {
                B = b;
            }
        }

        interface ID { }

        class CD : DisposeTracker, IC, ID
        {
            public A A;
            public B B;

            public CD(A a, B b)
            {
                A = a;
                B = b;
            }
        }

        class E : DisposeTracker
        {
            public B B;
            public IC C;

            public E(B b, IC c)
            {
                B = b;
                C = c;
            }
        }

        class F
        {
            public IList<A> AList;
            public F(IList<A> aList)
            {
                AList = aList;
            }
        }

        #endregion

        [Test]
        [ExpectedException(typeof(DependencyResolutionException))]
        public void InnerCannotResolveOuterDependencies()
        {
            var outer = new Container();
            outer.RegisterDelegate(c => new B(c.Resolve<A>())).SingleInstance();
            outer.RegisterDelegate(c => new C(c.Resolve<B>())).InstancePer("inner");
            outer.RegisterType<A>().InstancePer("inner");
            var inner = outer.BeginLifetimeScope();
            inner.Tag = "inner";
            var unused = inner.Resolve<C>();
        }

        [Test]
        public void OuterInstancesCannotReferenceInner()
        {
            var outer = new Container();

            outer.RegisterType<A>().InstancePerLifetimeScope();
            outer.RegisterDelegate(c => new B(c.Resolve<A>())).UnsharedInstances();

            var inner = outer.BeginLifetimeScope();

            var outerB = outer.Resolve<B>();
            var innerB = inner.Resolve<B>();
            var outerA = outer.Resolve<A>();
            var innerA = inner.Resolve<A>();

            Assert.AreSame(innerA, innerB.A);
            Assert.AreSame(outerA, outerB.A);
            Assert.AreNotSame(innerA, outerA);
            Assert.AreNotSame(innerB, outerB);
        }

        [Test]
        public void IntegrationTest()
        {
            var container = new Container();

            container.RegisterType<A>().SingleInstance();
            
            container.RegisterDelegate(ctr => new CD(ctr.Resolve<A>(), ctr.Resolve<B>()))
                .As<IC, ID>()
                .SingleInstance();
            
            container.RegisterDelegate(ctr => new E(ctr.Resolve<B>(), ctr.Resolve<IC>())).SingleInstance();

            container.RegisterDelegate(ctr => new B(ctr.Resolve<A>()))
                .UnsharedInstances();

            E e = container.Resolve<E>();
            A a = container.Resolve<A>();
            B b = container.Resolve<B>();
            IC c = container.Resolve<IC>();
            ID d = container.Resolve<ID>();

            Assert.IsInstanceOfType(typeof(CD), c);
            CD cd = (CD)c;

            Assert.AreSame(a, b.A);
            Assert.AreSame(a, cd.A);
            Assert.AreNotSame(b, cd.B);
            Assert.AreSame(c, e.C);
            Assert.AreNotSame(b, e.B);
            Assert.AreNotSame(e.B, cd.B);
        }

        [Test]
        public void DisposeOrder1()
        {
            var target = new Container();

            target.RegisterComponent(CreateRegistration(
                new ReflectionActivator(typeof(A)),
                new[] { new TypedService(typeof(A)) }));

            target.RegisterComponent(CreateRegistration(
                new DelegateActivator(typeof(B), (c,p) => new B(c.Resolve<A>())),
                new[] { new TypedService(typeof(B)) }));

            A a = target.Resolve<A>();
            B b = target.Resolve<B>();

            Queue<object> disposeOrder = new Queue<object>();

            a.Disposing += (s, e) => disposeOrder.Enqueue(a);
            b.Disposing += (s, e) => disposeOrder.Enqueue(b);

            target.Dispose();

            // B depends on A, therefore B should be disposed first

            Assert.AreEqual(2, disposeOrder.Count);
            Assert.AreSame(b, disposeOrder.Dequeue());
            Assert.AreSame(a, disposeOrder.Dequeue());
        }

        // In this version, resolve order is reversed.
        [Test]
        public void DisposeOrder2()
        {
            var target = new Container();

            target.RegisterComponent(CreateRegistration(
                new ReflectionActivator(typeof(A)),
                new[] { new TypedService(typeof(A)) }));

            target.RegisterComponent(CreateRegistration(
                new DelegateActivator(typeof(B), (c, p) => new B(c.Resolve<A>())),
                new[] { new TypedService(typeof(B)) }));

            B b = target.Resolve<B>();
            A a = target.Resolve<A>();

            Queue<object> disposeOrder = new Queue<object>();

            a.Disposing += (s, e) => disposeOrder.Enqueue(a);
            b.Disposing += (s, e) => disposeOrder.Enqueue(b);

            target.Dispose();

            // B depends on A, therefore B should be disposed first

            Assert.AreEqual(2, disposeOrder.Count);
            Assert.AreSame(b, disposeOrder.Dequeue());
            Assert.AreSame(a, disposeOrder.Dequeue());
        }

        [Test]
        public void ResolveSingletonFromContext()
        {
            var target = new Container();

            target.RegisterType<A>().SingleInstance();

            var context = target.BeginLifetimeScope();

            var ctxA = context.Resolve<A>();
            var targetA = target.Resolve<A>();

            Assert.AreSame(ctxA, targetA);
            Assert.IsNotNull(ctxA);

            Assert.IsFalse(ctxA.IsDisposed);

            context.Dispose();

            Assert.IsFalse(ctxA.IsDisposed);

            target.Dispose();

            Assert.IsTrue(ctxA.IsDisposed);
        }

        [Test]
        public void ResolveTransientFromContext()
        {
            var target = new Container();

            target.RegisterType<A>().UnsharedInstances();

            var context = target.BeginLifetimeScope();

            var ctxA = context.Resolve<A>();
            var targetA = target.Resolve<A>();

            Assert.IsNotNull(ctxA);
            Assert.IsNotNull(targetA);
            Assert.AreNotSame(ctxA, targetA);

            Assert.IsFalse(targetA.IsDisposed);
            Assert.IsFalse(ctxA.IsDisposed);

            context.Dispose();

            Assert.IsFalse(targetA.IsDisposed);
            Assert.IsTrue(ctxA.IsDisposed);

            target.Dispose();

            Assert.IsTrue(targetA.IsDisposed);
            Assert.IsTrue(ctxA.IsDisposed);
        }

        [Test]
        public void ResolveScopedFromContext()
        {
            var target = new Container();

            target.RegisterType<A>().InstancePerLifetimeScope();

            var context = target.BeginLifetimeScope();

            var ctxA = context.Resolve<A>();
            var ctxA2 = context.Resolve<A>();

            Assert.IsNotNull(ctxA);
            Assert.AreSame(ctxA, ctxA2);

            var targetA = target.Resolve<A>();
            var targetA2 = target.Resolve<A>();

            Assert.IsNotNull(targetA);
            Assert.AreSame(targetA, targetA2);
            Assert.AreNotSame(ctxA, targetA);

            Assert.IsFalse(targetA.IsDisposed);
            Assert.IsFalse(ctxA.IsDisposed);

            context.Dispose();

            Assert.IsFalse(targetA.IsDisposed);
            Assert.IsTrue(ctxA.IsDisposed);

            target.Dispose();

            Assert.IsTrue(targetA.IsDisposed);
            Assert.IsTrue(ctxA.IsDisposed);
        }

        //class ObjectRegistrationSource : IRegistrationSource
        //{
        //    public bool TryGetRegistration(Service service, out IComponentRegistration registration)
        //    {
        //        Assert.AreEqual(typeof(object), ((TypedService)service).ServiceType);
        //        registration = CreateRegistration(
        //            new[] { service },
        //            new ReflectionActivator(typeof(object)));
        //        return true;
        //    }
        //}

        //[Test]
        //public void AddRegistrationInServiceNotRegistered()
        //{
        //    var c = new Container();

        //    Assert.IsFalse(c.IsRegistered<object>());

        //    c.AddRegistrationSource(new ObjectRegistrationSource());

        //    Assert.IsTrue(c.IsRegistered<object>());

        //    var o = c.Resolve<object>();
        //    Assert.IsNotNull(o);
        //}

        //[Test]
        //public void ResolveByName()
        //{
        //    string name = "name";

        //    var r = CreateRegistration(
        //        new Service[] { new NamedService(name) },
        //        new ReflectionActivator(typeof(object)));

        //    var c = new Container();
        //    c.RegisterComponent(r);

        //    object o;

        //    Assert.IsTrue(c.TryResolve(name, out o));
        //    Assert.IsNotNull(o);

        //    Assert.IsFalse(c.IsRegistered<object>());
        //}

        //class DependsByCtor
        //{
        //    public DependsByCtor(DependsByProp o)
        //    {
        //        Dep = o;
        //    }

        //    public DependsByProp Dep { get; private set; }
        //}

        //class DependsByProp
        //{
        //    public DependsByCtor Dep { get; set; }
        //}

        //[Test]
        //public void CtorPropDependencyOkOrder1()
        //{
        //    var cb = new ContainerBuilder();
        //    cb.Register<DependsByCtor>();
        //    cb.Register<DependsByProp>()
        //        .OnActivated(ActivatedHandler.InjectProperties);

        //    var c = cb.Build();
        //    var dbp = c.Resolve<DependsByProp>();

        //    Assert.IsNotNull(dbp.Dep);
        //    Assert.IsNotNull(dbp.Dep.Dep);
        //    Assert.AreSame(dbp, dbp.Dep.Dep);
        //}

        //[Test]
        //public void CtorPropDependencyOkOrder2()
        //{
        //    var cb = new ContainerBuilder();
        //    cb.Register<DependsByCtor>();
        //    cb.Register<DependsByProp>()
        //        .OnActivated(ActivatedHandler.InjectProperties);

        //    var c = cb.Build();
        //    var dbc = c.Resolve<DependsByCtor>();

        //    Assert.IsNotNull(dbc.Dep);
        //    Assert.IsNotNull(dbc.Dep.Dep);
        //    Assert.AreSame(dbc, dbc.Dep.Dep);
        //}

        //[Test]
        //[ExpectedException(typeof(DependencyResolutionException))]
        //public void CtorPropDependencyFactoriesOrder1()
        //{
        //    var cb = new ContainerBuilder();
        //    using (cb.SetDefaultScope(InstanceScope.Factory))
        //    {
        //        cb.Register<DependsByCtor>();
        //        cb.Register<DependsByProp>()
        //            .OnActivated(ActivatedHandler.InjectProperties);
        //    }

        //    var c = cb.Build();
        //    var dbp = c.Resolve<DependsByProp>();
        //}

        //[Test]
        //[ExpectedException(typeof(DependencyResolutionException))]
        //public void CtorPropDependencyFactoriesOrder2()
        //{
        //    var cb = new ContainerBuilder();
        //    using (cb.SetDefaultScope(InstanceScope.Factory))
        //    {
        //        cb.Register<DependsByCtor>();
        //        cb.Register<DependsByProp>()
        //            .OnActivated(ActivatedHandler.InjectProperties);
        //    }

        //    var c = cb.Build();
        //    var dbc = c.Resolve<DependsByCtor>();
        //}

        //class Parameterised
        //{
        //    public string A { get; private set; }
        //    public int B { get; private set; }

        //    public Parameterised(string a, int b)
        //    {
        //        A = a;
        //        B = b;
        //    }
        //}

        //[Test]
        //public void RegisterParameterisedWithDelegate()
        //{
        //    var cb = new ContainerBuilder();
        //    cb.Register((c, p) => new Parameterised(p.Get<string>("a"), p.Get<int>("b")));
        //    var container = cb.Build();
        //    var aVal = "Hello";
        //    var bVal = 42;
        //    var result = container.Resolve<Parameterised>(
        //        new Parameter("a", aVal),
        //        new Parameter("b", bVal));
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(aVal, result.A);
        //    Assert.AreEqual(bVal, result.B);
        //}

        //[Test]
        //public void RegisterParameterisedWithReflection()
        //{
        //    var cb = new ContainerBuilder();
        //    cb.Register<Parameterised>();
        //    var container = cb.Build();
        //    var aVal = "Hello";
        //    var bVal = 42;
        //    var result = container.Resolve<Parameterised>(
        //        new Parameter("a", aVal),
        //        new Parameter("b", bVal));
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(aVal, result.A);
        //    Assert.AreEqual(bVal, result.B);
        //}

        //[Test]
        //public void SupportsIServiceProvider()
        //{
        //    var cb = new ContainerBuilder();
        //    cb.Register<object>();
        //    var container = cb.Build();
        //    var sp = (IServiceProvider)container;
        //    var o = sp.GetService(typeof(object));
        //    Assert.IsNotNull(o);
        //    var s = sp.GetService(typeof(string));
        //    Assert.IsNull(s);
        //}

        //[Test]
        //public void ResolveByNameWithServiceType()
        //{
        //    var myName = "Something";
        //    var cb = new ContainerBuilder();
        //    cb.Register<object>().Named(myName);
        //    var container = cb.Build();
        //    var o = container.Resolve<object>(myName);
        //    Assert.IsNotNull(o);
        //}

        //[Test]
        //public void ComponentRegistrationsExposed()
        //{
        //    var builder = new ContainerBuilder();
        //    builder.Register<object>();
        //    builder.Register<object>();
        //    builder.Register("hello");
        //    var container = builder.Build();
        //    var registrations = new List<IComponentRegistration>(container.ComponentRegistrations);
        //    // The container registers itself :) hence 3 + 1.
        //    Assert.AreEqual(4, registrations.Count);
        //    Assert.IsTrue(registrations[0].Descriptor.Services.Contains(new TypedService(typeof(IContainer))));
        //    Assert.IsTrue(registrations[1].Descriptor.Services.Contains(new TypedService(typeof(object))));
        //    Assert.IsTrue(registrations[2].Descriptor.Services.Contains(new TypedService(typeof(object))));
        //    Assert.IsTrue(registrations[3].Descriptor.Services.Contains(new TypedService(typeof(string))));
        //}

        //[Test]
        //public void ComponentRegisteredEventFired()
        //{
        //    object eventSender = null;
        //    ComponentRegisteredEventArgs args = null;
        //    var eventCount = 0;

        //    var container = new Container();
        //    container.ComponentRegistered += (sender, e) =>
        //    {
        //        eventSender = sender;
        //        args = e;
        //        ++eventCount;
        //    };

        //    var builder = new ContainerBuilder();
        //    builder.Register<object>();
        //    builder.Build(container);

        //    Assert.AreEqual(1, eventCount);
        //    Assert.IsNotNull(eventSender);
        //    Assert.AreSame(container, eventSender);
        //    Assert.IsNotNull(args);
        //    Assert.AreSame(container, args.Container);
        //    Assert.IsNotNull(args.ComponentRegistration.Descriptor.Services.FirstOrDefault(
        //        s => s == new TypedService(typeof(object))));
        //}

        //[Test]
        //public void ComponentRegisteredNotFiredOnNewContext()
        //{
        //    var eventCount = 0;

        //    var container = new Container();
        //    container.ComponentRegistered += (sender, e) =>
        //    {
        //        ++eventCount;
        //    };

        //    var builder = new ContainerBuilder();
        //    builder.Register<object>().ContainerScoped();
        //    builder.Build(container);

        //    var inner = container.CreateInnerContainer();
        //    inner.Resolve<object>();

        //    Assert.AreEqual(1, eventCount);
        //}

        //[Test]
        //public void DefaultRegistrationIsForMostRecent()
        //{
        //    var builder = new ContainerBuilder();
        //    builder.Register<object>().As<object>().Named("first");
        //    builder.Register<object>().As<object>().Named("second");
        //    var container = builder.Build();

        //    IComponentRegistration defaultRegistration;
        //    Assert.IsTrue(container.TryGetDefaultRegistrationFor(new TypedService(typeof(object)), out defaultRegistration));
        //    Assert.IsTrue(defaultRegistration.Descriptor.Services.Contains(new NamedService("second")));
        //}

        //[Test]
        //public void DefaultRegistrationFalseWhenAbsent()
        //{
        //    var container = new Container();
        //    IComponentRegistration unused;
        //    Assert.IsFalse(container.TryGetDefaultRegistrationFor(new TypedService(typeof(object)), out unused));
        //}

        //[Test]
        //public void DefaultRegistrationSuppliedDynamically()
        //{
        //    var container = new Container();
        //    container.AddRegistrationSource(new ObjectRegistrationSource());
        //    IComponentRegistration registration;
        //    Assert.IsTrue(container.TryGetDefaultRegistrationFor(new TypedService(typeof(object)), out registration));
        //}

        //[Test]
        //public void IdSameInSubcontext()
        //{
        //    var builder = new ContainerBuilder();
        //    builder.Register<object>().ContainerScoped();

        //    var container = builder.Build();
        //    IComponentRegistration r1;
        //    Assert.IsTrue(container.TryGetDefaultRegistrationFor(new TypedService(typeof(object)), out r1));

        //    var inner = container.CreateInnerContainer();
        //    IComponentRegistration r2;
        //    Assert.IsTrue(inner.TryGetDefaultRegistrationFor(new TypedService(typeof(object)), out r2));

        //    Assert.AreNotSame(r1, r2);
        //    Assert.AreEqual(r1.Descriptor.Id, r2.Descriptor.Id);
        //}
    }
}
