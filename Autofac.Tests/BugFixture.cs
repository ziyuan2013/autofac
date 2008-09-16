using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using Autofac.Registry;
using Autofac.Services;

namespace Autofac.Tests
{
    /// <summary>
    /// This should be called the 'paranoia fixture' - suspicions get aired here.
    /// </summary>
    [TestFixture]
    public class BugFixture
    {
        interface IA { }

        interface IB { }

        interface IC { }

        interface ID { }

        class A : IA
        {
            public A(IC c) { }
        }

        class BC : IB, IC
        {
            public BC(IA a) { }
        }

        class D : ID
        {
            public D(IA a, IC c) { }
        }

        [Test]
        public void CorrectExceptionThrownOnIndirectCircularity()
        {
            try
            {
                var container = new Container();
                container.RegisterType<D>().As<ID>();
                container.RegisterType<A>().As<IA>();
                container.RegisterType<BC>().As<IB, IC>();

                ID d = container.Resolve<ID>();

                Assert.Fail("Expected circular dependency exception.");
            }
            catch (DependencyResolutionException de)
            {
                Assert.IsTrue(de.Message.Contains(
                    "Autofac.Tests.BugFixture+D -> Autofac.Tests.BugFixture+A -> Autofac.Tests.BugFixture+BC -> Autofac.Tests.BugFixture+A"));
            }
            catch (Exception ex)
            {
                Assert.Fail("Wrong exception type caught: " + ex.ToString());
            }
        }

        [Test]
        public void CanCheckRegistrationStatusFromInnerScope()
        {
            var container = new Container();
            var inner = container.BeginLifetimeScope();
            Assert.IsFalse(inner.IsRegistered<string>());
        }

        [Test]
        public void DisposesProvidedInstancesEvenWhenNeverResolved()
        {
            var container = new Container();
            var disposable = new DisposeTracker();
            container.RegisterInstance(disposable);
            container.Dispose();
            Assert.IsTrue(disposable.IsDisposed);
        }

        [Test]
        public void DefaultMaintainedInSubcontext()
        {
            var a = new object();
            var b = new object();
            var container = new Container();
            container.RegisterDelegate(c => a).Named("a").Named("other").UnsharedInstances();
            container.RegisterDelegate(c => b).Named("b").Named("other").UnsharedInstances();
            
            var outerOther = container.Resolve("other");
            Assert.AreSame(b, outerOther);
            var inner = container.BeginLifetimeScope();
            var innerOtherOne = inner.Resolve("other");
            Assert.AreSame(b, innerOtherOne);
            inner.Resolve("a");
            var innerOtherTwo = inner.Resolve("other");
            Assert.AreSame(b, innerOtherTwo);
        }

        [Test]
        public void DefaultMaintainedInSubcontext2()
        {
            var a = new object();
            var b = new object();
            var container = new Container();
            container.RegisterDelegate(c => a).Named("a").Named("other").UnsharedInstances();
            container.RegisterDelegate(c => b).Named("b").Named("other").UnsharedInstances();
            
            var outerOther = container.Resolve("other");
            Assert.AreSame(b, outerOther);
            var inner = container.BeginLifetimeScope();
            inner.Resolve("a");
            var innerOther = inner.Resolve("other");
            Assert.AreSame(b, innerOther);
        }

        [Test]
        public void DefaultMaintainedInSubcontext3()
        {
            var container = new Container();
            container.RegisterType<object>().Named("a").Named("other").UnsharedInstances();
            container.RegisterType<object>().Named("b").Named("other").UnsharedInstances();
            
            var inner = container.BeginLifetimeScope();
            IComponentRegistration cr1, cr2;
            inner.ComponentRegistry.TryGetRegistration(new NamedService("other"), out cr1);
            inner.Resolve("a");
            inner.ComponentRegistry.TryGetRegistration(new NamedService("other"), out cr2);
            Assert.AreSame(cr1, cr2);
        }

        //[Test]
        //public void RegisteringChangesDefaultInSubcontext()
        //{
        //    var container = new Container();
        //    container.RegisterType<object>().Named("a").Named("other").UnsharedInstances();
            
        //    var inner = container.BeginLifetimeScope();
        //    IComponentRegistration cr1, cr2;
        //    inner.Resolve("a");
        //    inner.ComponentRegistry.TryGetRegistration(new NamedService("other"), out cr1);
        //    var innercontainer = container.BeginLifetimeScope();
        //    innercontainer.RegisterType<object>().Named("other");
        //    inner.ComponentRegistry.TryGetRegistration(new NamedService("other"), out cr2);
        //    Assert.AreNotSame(cr1, cr2);
        //}

        [Test]
        public void MultipleServicesDoNotResultInMultipleRegistrations()
        {
            var container = new Container();
            container.RegisterType<object>().Named("a").Named("b").InstancePerLifetimeScope();
            
            var inner = container.BeginLifetimeScope();
            inner.Resolve("a");
            var count = container.ComponentRegistry.Registrations.Count();
            inner.Resolve("b");
            Assert.AreEqual(count, container.ComponentRegistry.Registrations.Count());
        }

        //[Test]
        //public void MultipleServicesResultInMultipleRegistrationsGeneric()
        //{
        //    var container = new Container();
        //    container.RegisterGeneric(typeof(List<>)).As(typeof(IEnumerable<>), typeof(ICollection<>)).ContainerScoped().UsingConstructor();
            
        //    var inner = container.BeginLifetimeScope();
        //    inner.Resolve<IEnumerable<int>>();
        //    var count = inner.ComponentRegistrations.Count();
        //    inner.Resolve<ICollection<int>>();
        //    Assert.AreEqual(count, inner.ComponentRegistrations.Count());
        //}

        //[Test]
        //public void ResolvingChangesDefaultInSubcontext3Generic()
        //{
        //    var container = new Container();
        //    container.SetDefaultScope(InstanceScope.Factory);
        //    container.RegisterGeneric(typeof(List<>)).As(typeof(IList<>), typeof(ICollection<>)).UsingConstructor();
        //    container.RegisterGeneric(typeof(List<>)).As(typeof(IEnumerable<>), typeof(ICollection<>)).UsingConstructor();
            
        //    var inner = container.BeginLifetimeScope();
        //    IComponentRegistration cr1, cr2;
        //    inner.ComponentRegistry.TryGetRegistration(new TypedService(typeof(ICollection<int>)), out cr1);
        //    inner.Resolve<IList<int>>();
        //    inner.ComponentRegistry.TryGetRegistration(new TypedService(typeof(ICollection<int>)), out cr2);
        //    Assert.AreSame(cr1, cr2);
        //}
    }
}
