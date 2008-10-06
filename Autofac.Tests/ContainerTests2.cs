using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Autofac.Tests
{
    // Came from ContainerBuilderFixture - will be refactored

    [TestFixture]
    public class ContainerTests2
    {
        interface IA { }
        interface IB { }
        interface IC { }

        class Abc : DisposeTracker, IA, IB, IC { }

        [Test]
        public void CanResolveType()
        {
            var c = new Container();
            c.RegisterType<Abc>();
            var a = c.Resolve<Abc>();
            Assert.IsNotNull(a);
            Assert.IsInstanceOfType(typeof(Abc), a);
        }

        [Test]
        public void CanResolveTypeThroughInterface()
        {
            var c = new Container();
            c.RegisterType<Abc>().As<IA>();
            var a = c.Resolve<IA>();
            Assert.IsNotNull(a);
            Assert.IsInstanceOfType(typeof(Abc), a);
            Assert.IsFalse(c.IsRegistered<Abc>());
        }

        [Test]
        public void UnsharedInstancesAlwaysCreateNew()
        {
            var c = new Container();
            c.RegisterType<Abc>().UnsharedInstances();

            var a1 = c.Resolve<Abc>();
            var a2 = c.Resolve<Abc>();

            Assert.IsNotNull(a1);
            Assert.AreNotSame(a1, 12);
        }

        [Test]
        public void ExternalOwnershipPreventsDisposal()
        {
            var c = new Container();
            c.RegisterType<Abc>().ExternallyOwned();

            var a1 = c.Resolve<Abc>();
            c.Dispose();

            Assert.IsFalse(a1.IsDisposed);
        }

        [Test]
        public void SingleInstanceComponentsReturnSame()
        {
            var c = new Container();
            c.RegisterType<Abc>().SingleInstance();

            var a1 = c.Resolve<Abc>();
            var a2 = c.Resolve<Abc>();

            Assert.IsNotNull(a1);
            Assert.AreSame(a1, a2);
        }

        [Test]
        public void SingletonDisposedWhenContainerDisposed()
        {
            var c = new Container();
            c.RegisterType<Abc>().SingleInstance();

            var a1 = c.Resolve<Abc>();

            c.Dispose();

            Assert.IsTrue(a1.IsDisposed);
        }

        [Test]
        public void UnsharedInstancesAreDisposedWithNestedLifetimeScope()
        {
            var c = new Container();
            c.RegisterType<Abc>().UnsharedInstances();

            var ctx = c.BeginLifetimeScope();
            var a1 = ctx.Resolve<Abc>();
            ctx.Dispose();

            Assert.IsTrue(a1.IsDisposed);
        }

        [Test]
        public void LastRegisteredInstancesActsAsDefault()
        {
            var target = new Container();
            var inst1 = new object();
            var inst2 = new object();
            target.RegisterInstance(inst1);
            target.RegisterInstance(inst2);
            Assert.AreSame(inst2, target.Resolve<object>());
        }

        class ObjectModule : IModule
        {
            public bool ConfigureCalled { get; private set; }

            #region IModule Members

            public void Configure(IContainer container)
            {
                if (container == null) throw new ArgumentNullException("container");
                ConfigureCalled = true;
                container.RegisterInstance(new object()).InstancePerLifetimeScope();
            }

            #endregion
        }

        [Test]
        public void RegisteringModuleConfiguresContainer()
        {
            var mod = new ObjectModule();
            Assert.IsFalse(mod.ConfigureCalled);
            var target = new Container();
            target.RegisterModule(mod);
            Assert.IsTrue(mod.ConfigureCalled);
            Assert.IsTrue(target.IsRegistered<object>());
        }

        class A1 { }
        class A2 { }

        //[Test]
        //public void DefaultScopeChanged()
        //{
        //    var container = new Container();
        //    using (container.SetDefaultScope(InstanceScope.Factory))
        //    {
        //        using (container.SetDefaultScope(InstanceScope.Singleton))
        //        {
        //            // Should have been changed to Singleton
        //            container.RegisterType<A2>();
        //        }

        //        // Should revert to Factory
        //        container.RegisterType<A1>();
        //    }


        //    Assert.AreNotSame(container.Resolve<A1>(), container.Resolve<A1>());
        //    Assert.AreSame(container.Resolve<A2>(), container.Resolve<A2>());
        //}

        //[Test]
        //public void DefaultOwnershipChanged()
        //{
        //    var contextOwnercontainer = new Container();
        //    using (contextOwnercontainer.SetDefaultOwnership(InstanceOwnership.Container))
        //    {
        //        var disposable = new DisposeTracker();
        //        contextOwnercontainer.RegisterInstance(disposable);
        //        var container = contextOwnercontainer;
        //        container.Resolve<DisposeTracker>();
        //        container.Dispose();
        //        Assert.IsTrue(disposable.IsDisposed);
        //    }

        //    var nonOwnedcontainer = new Container();
        //    using (nonOwnedcontainer.SetDefaultOwnership(InstanceOwnership.External))
        //    {
        //        var notDisposed = new DisposeTracker();
        //        nonOwnedcontainer.RegisterInstance(notDisposed);
        //        var container = nonOwnedcontainer;
        //        container.Resolve<DisposeTracker>();
        //        container.Dispose();
        //        Assert.IsFalse(notDisposed.IsDisposed);
        //    }
        //}

        public class Named
        {
            public delegate Named Factory(string name);

            public string Name { get; set; }

            public Named(string name, object o)
            {
                Name = name;
            }
        }

        [Test]
        public void CanResolveInstancesByName()
        {
            var name = "object.registration";

            var c = new Container();
            c.RegisterType<object>().Named(name);

            object o1;
            Assert.IsTrue(c.TryResolve(name, out o1));
            Assert.IsNotNull(o1);
        }

        [Test]
        public void WhenRegisteringByNameTypeNotProvidesAsServiceByDefault()
        {
            var name = "object.registration";

            var c = new Container();
            c.RegisterType<object>().Named(name);

            object o2;
            Assert.IsFalse(c.TryResolve(typeof(object), out o2));
        }

        //[Test]
        //public void WithExtendedProperties()
        //{
        //    var p1 = new KeyValuePair<string, object>("p1", "p1Value");
        //    var p2 = new KeyValuePair<string, object>("p2", "p2Value");

        //    var container = new Container();
        //    container.RegisterType<object>()
        //        .WithExtendedProperty(p1.Key, p1.Value)
        //        .WithExtendedProperty(p2.Key, p2.Value);



        //    IComponentRegistration registration;
        //    Assert.IsTrue(container.TryGetDefaultRegistrationFor(new TypedService(typeof(object)), out registration));

        //    Assert.AreEqual(2, registration.Descriptor.ExtendedProperties.Count);
        //    Assert.IsTrue(registration.Descriptor.ExtendedProperties.Contains(p1));
        //    Assert.IsTrue(registration.Descriptor.ExtendedProperties.Contains(p2));
        //}

        //[Test]
        //public void FiresPreparing()
        //{
        //    int preparingFired = 0;
        //    var c = new Container();
        //    c.RegisterType<object>().OnPreparing((s, e) => ++preparingFired);
        //    var container = cb;
        //    container.Resolve<object>();
        //    Assert.AreEqual(1, preparingFired);
        //}

        class Module1 : Module
        {
            protected override void Load(IContainer container)
            {
                base.Load(container);
                container.RegisterType<object>();
            }
        }

        class Module2 : Module
        {
            protected override void Load(IContainer container)
            {
                base.Load(container);
                container.RegisterModule(new Module1());
            }
        }

        [Test]
        public void ModuleCanRegisterModule()
        {
            var container = new Container();
            container.RegisterModule(new Module2());
            Assert.IsTrue(container.IsRegistered<object>());
        }
    }
}
