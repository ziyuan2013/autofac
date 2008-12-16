using NUnit.Framework;
using System;
using Autofac.Activators;
using System.Collections.Generic;
using Autofac.Registry;
using Autofac.Lifetime;
using Autofac.Services;
using Autofac.Disposal;
using Autofac.RegistrationSources;
using System.Linq;
using Autofac.OwnedInstances;
using Autofac.GeneratedFactories;

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
        public void DisposeOrderIsInverseOfDependencyDirection1()
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
        public void DisposeOrderIsInverseOfDependencyDirection2()
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
        public void ResolveSingletonFromNestedScopeGivesSameInstance()
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
        public void ResolveTransientFromNestedScopeGivesNewInstance()
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
        public void ResolveLifetimeScopeBoundFromNestedScopeGivesNewInstance()
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

        class ObjectRegistrationSource : IDynamicRegistrationSource
        {
            public bool TryGetRegistration(Service service, Func<Service, bool> registeredServicesTest, out IComponentRegistration registration)
            {
                Assert.AreEqual(typeof(object), ((TypedService)service).ServiceType);
                registration = CreateRegistration(
                    new ReflectionActivator(typeof(object)),
                    new[] { service });
                return true;
            }
        }

        [Test]
        public void RegistrationsAreAddedFromDynamicRegistrationSource()
        {
            var c = new Container();

            Assert.IsFalse(c.IsRegistered<object>());

            c.ComponentRegistry.AddDynamicRegistrationSource(new ObjectRegistrationSource());

            Assert.IsTrue(c.IsRegistered<object>());

            var o = c.Resolve<object>();
            Assert.IsNotNull(o);
        }

        [Test]
        public void CanResolveByName()
        {
            string name = "name";

            var r = CreateRegistration(
                new ReflectionActivator(typeof(object)),
                new Service[] { new NamedService(name) });

            var c = new Container();
            c.RegisterComponent(r);

            object o;

            Assert.IsTrue(c.TryResolve(name, out o));
            Assert.IsNotNull(o);

            Assert.IsFalse(c.IsRegistered<object>());
        }

        class DependsByCtor
        {
            public DependsByCtor(DependsByProp o)
            {
                Dep = o;
            }

            public DependsByProp Dep { get; private set; }
        }

        class DependsByProp
        {
            public DependsByCtor Dep { get; set; }
        }

        [Test]
        public void CtorPropDependencyOkOrder1()
        {
            var c = new Container();
            c.RegisterType<DependsByCtor>().SingleInstance();
            c.RegisterType<DependsByProp>().SingleInstance().PropertiesAutowired(true);

            var dbp = c.Resolve<DependsByProp>();

            Assert.IsNotNull(dbp.Dep);
            Assert.IsNotNull(dbp.Dep.Dep);
            Assert.AreSame(dbp, dbp.Dep.Dep);
        }

        [Test]
        public void CtorPropDependencyOkOrder2()
        {
            var c = new Container();
            c.RegisterType<DependsByCtor>().SingleInstance();
            c.RegisterType<DependsByProp>().SingleInstance().PropertiesAutowired(true);

            var dbc = c.Resolve<DependsByCtor>();

            Assert.IsNotNull(dbc.Dep);
            Assert.IsNotNull(dbc.Dep.Dep);
            Assert.AreSame(dbc, dbc.Dep.Dep);
        }

        [Test]
        [ExpectedException(typeof(DependencyResolutionException))]
        public void CtorPropDependencyFactoriesOrder1()
        {
            var c = new Container();
            c.RegisterType<DependsByCtor>().UnsharedInstances();
            c.RegisterType<DependsByProp>().UnsharedInstances().PropertiesAutowired(true);
            var dbp = c.Resolve<DependsByProp>();
        }

        [Test]
        [ExpectedException(typeof(DependencyResolutionException))]
        public void CtorPropDependencyFactoriesOrder2()
        {
            var c = new Container();
            c.RegisterType<DependsByCtor>().UnsharedInstances();
            c.RegisterType<DependsByProp>().UnsharedInstances().PropertiesAutowired(true);

            var dbc = c.Resolve<DependsByCtor>();
        }

        class Parameterised
        {
            public string A { get; private set; }
            public int B { get; private set; }

            public Parameterised(string a, int b)
            {
                A = a;
                B = b;
            }
        }

        [Test]
        public void CanExplicitlyRetrieveNamedParameters()
        {
            var container = new Container();
            container.RegisterDelegate(
                (c, p) => new Parameterised(p.Named<string>("a"), p.Named<int>("b")));
            var aVal = "Hello";
            var bVal = 42;
            var result = container.Resolve<Parameterised>(
                new Parameter[] { 
                    new NamedParameter("a", aVal),
                    new NamedParameter("b", bVal)});
            Assert.IsNotNull(result);
            Assert.AreEqual(aVal, result.A);
            Assert.AreEqual(bVal, result.B);
        }

        [Test]
        public void CanExplicitlyRetrievePositionalParameters()
        {
            var container = new Container();
            container.RegisterDelegate(
                (c, p) => new Parameterised(p.Positional<string>(0), p.Positional<int>(1)));
            var aVal = "Hello";
            var bVal = 42;
            var result = container.Resolve<Parameterised>(
                new Parameter[] { 
                    new PositionalParameter(0, aVal),
                    new PositionalParameter(1, bVal)});
            Assert.IsNotNull(result);
            Assert.AreEqual(aVal, result.A);
            Assert.AreEqual(bVal, result.B);
        }

        [Test]
        public void CanRetrieveNamedParametersViaReflection()
        {
            var container = new Container();
            container.RegisterType<Parameterised>();
            var aVal = "Hello";
            var bVal = 42;
            var result = container.Resolve<Parameterised>(
                new Parameter[] { 
                    new NamedParameter("a", aVal),
                    new NamedParameter("b", bVal)});
            Assert.IsNotNull(result);
            Assert.AreEqual(aVal, result.A);
            Assert.AreEqual(bVal, result.B);
        }

        [Test]
        public void CanRetrieveMixedParametersViaReflection()
        {
            var container = new Container();
            container.RegisterType<Parameterised>();
            var aVal = "Hello";
            var bVal = 42;
            var result = container.Resolve<Parameterised>(
                new Parameter[] {
                    new PositionalParameter(1, bVal),
                    new NamedParameter("a", aVal)});
            Assert.IsNotNull(result);
            Assert.AreEqual(aVal, result.A);
            Assert.AreEqual(bVal, result.B);
        }

        [Test]
        public void CanResolveByNameWithGenericTypeArgument()
        {
            var myName = "Something";
            var container = new Container();
            container.RegisterType<object>().Named(myName);
            var o = container.Resolve<object>(myName);
            Assert.IsNotNull(o);
        }

        [Test]
        public void ComponentRegistrationsExposed()
        {
            var container = new Container();
            container.RegisterType<object>();
            container.RegisterType<object>();
            container.RegisterInstance("hello");
            var registrations = new List<IComponentRegistration>(container.ComponentRegistry.Registrations);

            // The container registers itself using a bit of indirection.
            Assert.AreEqual(5, registrations.Count);

            // zero is unimportant

            Assert.IsTrue(registrations[1].Services.Contains(new TypedService(typeof(IComponentContext))));
            Assert.IsTrue(registrations[1].Services.Contains(new TypedService(typeof(ILifetimeScope))));

            Assert.IsTrue(registrations[2].Services.Contains(new TypedService(typeof(object))));
            Assert.IsTrue(registrations[3].Services.Contains(new TypedService(typeof(object))));
            Assert.IsTrue(registrations[4].Services.Contains(new TypedService(typeof(string))));
        }

        [Test]
        public void ComponentRegisteredEventFired()
        {
            object eventSender = null;
            ComponentRegisteredEventArgs args = null;
            var eventCount = 0;

            var container = new Container();
            container.ComponentRegistry.Registered += (sender, e) =>
            {
                eventSender = sender;
                args = e;
                ++eventCount;
            };

            container.RegisterType<object>();

            // This is necessary because actual registration is deferred.
            Assert.IsTrue(container.IsRegistered<object>());

            Assert.AreEqual(1, eventCount);
            Assert.IsNotNull(eventSender);
            Assert.AreSame(container.ComponentRegistry, eventSender);
            Assert.IsNotNull(args);
            Assert.AreSame(container.ComponentRegistry, args.ComponentRegistry);
            Assert.IsTrue(args.ComponentRegistration.Services.Contains(new TypedService(typeof(object))));
        }

        [Test]
        public void ComponentRegisteredNotFiredOnNewContext()
        {
            var eventCount = 0;

            var container = new Container();
            container.ComponentRegistry.Registered += (sender, e) =>
            {
                ++eventCount;
            };

            container.RegisterType<object>().InstancePerLifetimeScope();

            var inner = container.BeginLifetimeScope();
            inner.Resolve<object>();

            Assert.AreEqual(1, eventCount);
        }

        [Test]
        public void DefaultRegistrationIsForMostRecent()
        {
            var container = new Container();
            container.RegisterType<object>().As<object>().Named("first");
            container.RegisterType<object>().As<object>().Named("second");

            IComponentRegistration defaultRegistration;
            Assert.IsTrue(container.ComponentRegistry.TryGetRegistration(new TypedService(typeof(object)), out defaultRegistration));
            Assert.IsTrue(defaultRegistration.Services.Contains(new NamedService("second")));
        }

        [Test]
        public void DefaultRegistrationFalseWhenAbsent()
        {
            var registry = new ComponentRegistry();
            IComponentRegistration unused;
            Assert.IsFalse(registry.TryGetRegistration(new TypedService(typeof(object)), out unused));
        }

        [Test]
        public void DefaultRegistrationSuppliedDynamically()
        {
            var registry = new ComponentRegistry();
            registry.AddDynamicRegistrationSource(new ObjectRegistrationSource());
            IComponentRegistration registration;
            Assert.IsTrue(registry.TryGetRegistration(new TypedService(typeof(object)), out registration));
        }

        [Test]
        public void ActivatingEventFiredInCorrectContext()
        {
            var container = new Container();

            container.RegisterType<object>().InstancePerLifetimeScope();

            object fromContext = null;
            container.RegisterDelegate(c => "hello")
                .SingleInstance()
                .OnActivating(e => { fromContext = e.Context.Resolve<object>(); });

            var lifetime = container.BeginLifetimeScope();
            var hello = lifetime.Resolve<string>();

            // The context in Activated should be the root one, as hello
            // is a singleton - not the nested one.
            Assert.AreSame(container.Resolve<object>(), fromContext);
        }

        [Test]
        public void ActivatingEventFiredInCorrectContext2()
        {
            var container = new Container();

            container.RegisterType<object>().InstancePerLifetimeScope();

            object fromContext = null;
            container.RegisterDelegate(c => "hello")
                .UnsharedInstances()
                .OnActivating(e => { fromContext = e.Context.Resolve<object>(); });

            var lifetime = container.BeginLifetimeScope();
            var hello = lifetime.Resolve<string>();

            // The context in Activated should be the root one, as hello
            // is a singleton - not the nested one.
            Assert.AreSame(lifetime.Resolve<object>(), fromContext);
        }

        [Test]
        public void ActivatedEventFiredInCorrectContext()
        {
            var container = new Container();

            container.RegisterType<object>().InstancePerLifetimeScope();

            object fromContext = null;
            container.RegisterDelegate(c => "hello")
                .SingleInstance()
                .OnActivated(e => { fromContext = e.Context.Resolve<object>(); });

            var lifetime = container.BeginLifetimeScope();
            var hello = lifetime.Resolve<string>();

            // The context in Activated should be the root one, as hello
            // is a singleton - not the nested one.
            Assert.AreSame(container.Resolve<object>(), fromContext);
        }

        [Test]
        public void ActivatedEventFiredInCorrectContext2()
        {
            var container = new Container();

            container.RegisterType<object>().InstancePerLifetimeScope();

            object fromContext = null;
            container.RegisterDelegate(c => "hello")
                .UnsharedInstances()
                .OnActivated(e => { fromContext = e.Context.Resolve<object>(); });

            var lifetime = container.BeginLifetimeScope();
            var hello = lifetime.Resolve<string>();

            // The context in Activated should be the root one, as hello
            // is a singleton - not the nested one.
            Assert.AreSame(lifetime.Resolve<object>(), fromContext);
        }

        [Test]
        public void CanResolveIComponentContextFromContainer()
        {
            var container = new Container();
            var cc = container.Resolve<IComponentContext>();
            Assert.IsNotNull(cc);
        }

        [Test]
        public void UnsharedInstancesDisposedAlongWithLifetimeScope()
        {
            var container = new Container();
            container.RegisterType<DisposeTracker>().UnsharedInstances();

            var lifetime = container.BeginLifetimeScope();

            var component = lifetime.Resolve<DisposeTracker>();

            Assert.IsFalse(component.IsDisposed);

            lifetime.Dispose();

            Assert.IsTrue(component.IsDisposed);
        }

        [Test]
        public void CanCombineGenerativeRegistrationMechanisms()
        {
            var c = new Container();
            c.ComponentRegistry.AddDynamicRegistrationSource(new OwnedRegistrationSource());
            c.ComponentRegistry.AddDynamicRegistrationSource(new AutoGeneratedFactoryRegistrationSource());
            c.RegisterType<DisposeTracker>();

            var instance = c.Resolve<Func<Owned<DisposeTracker>>>();

            var product = instance.Invoke();

            Assert.IsNotNull(product);
        }
    }
}
