using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Autofac.Tests
{
    [TestFixture]
    public class LifetimeTests
    {
        [Test]
        public void ResolvingLifetimeScopeInNestedLifetimeGivesCurrent()
        {
            var container = new Container();
            var lifetime = container.BeginLifetimeScope();
            var resolved = lifetime.Resolve<ILifetimeScope>();
            Assert.AreSame(lifetime, resolved);
            Assert.AreNotSame(container, resolved);
        }

        [Test]
        public void CurrentScopeInstanceResolvedFromInnerScopeDisposedWithIt()
        {
            var container = new Container();
            container.RegisterType<DisposeTracker>().UnsharedInstances();
            var lifetime = container.BeginLifetimeScope();
            var dt = lifetime.Resolve<DisposeTracker>();
            Assert.IsFalse(dt.IsDisposed);
            lifetime.Dispose();
            Assert.IsTrue(dt.IsDisposed);
        }
    }
}
