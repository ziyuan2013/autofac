using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Autofac.OwnedInstances;

namespace Autofac.Tests
{
    [TestFixture]
    public class OwnedInstancesTests
    {
        [Test]
        public void ResolvesOwnedInstances()
        {
            var c = new Container();
            c.ComponentRegistry.AddDynamicRegistrationSource(new OwnedRegistrationSource());
            c.RegisterType<DisposeTracker>().UnsharedInstances();
            var owned = c.Resolve<Owned<DisposeTracker>>();
            Assert.IsNotNull(owned.Value);
        }

        [Test]
        public void DisposesOwnedButDoesNotDisposeCurrentLifetimeScope()
        {
            var c = new Container();
            var containerDisposeTracker = new DisposeTracker();
            c.RegisterInstance(containerDisposeTracker).Named("tracker");
            c.ComponentRegistry.AddDynamicRegistrationSource(new OwnedRegistrationSource());
            c.RegisterType<DisposeTracker>().UnsharedInstances();
            var owned = c.Resolve<Owned<DisposeTracker>>();
            var dt = owned.Value;
            Assert.IsFalse(dt.IsDisposed);
            owned.Dispose();
            Assert.IsTrue(dt.IsDisposed);
            Assert.IsFalse(containerDisposeTracker.IsDisposed);
        }

        [Test]
        public void IfInnerTypeIsNotRegisteredOwnedTypeIsNotEither()
        {
            var c = new Container();
            c.ComponentRegistry.AddDynamicRegistrationSource(new OwnedRegistrationSource());
            Assert.IsFalse(c.IsRegistered<Owned<Object>>());
        }
    }
}
