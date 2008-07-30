using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Autofac.Tests
{
    [TestFixture]
    public class ContainerTests
    {
        [Test]
        public void CanRegisterAndResolveInstance()
        {
            var instance = new object();
            var container = new Container();
            container.RegisterInstance(instance);
            var resolved = container.Resolve<object>();
            Assert.AreEqual(instance, resolved);
        }
    }
}
