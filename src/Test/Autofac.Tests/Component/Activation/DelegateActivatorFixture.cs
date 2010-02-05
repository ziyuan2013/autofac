using System;
using Autofac.Component.Activation;
using NUnit.Framework;
using System.Linq;

namespace Autofac.Tests.Component.Activation
{
    [TestFixture]
    public class DelegateActivatorFixture
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorDoesNotAcceptNull()
        {
            new DelegateActivator(null);
        }

        [Test]
        public void ActivateInstanceReturnsResultOfCallingDelegate()
        {
            var instance = new object();

            var target =
                new DelegateActivator((c, p) => instance);

            Assert.AreSame(instance, target.ActivateInstance(new Container(), Enumerable.Empty<Parameter>()));
        }

        [Test]
        public void WhenDelegateReturnsNullImplementorIsIncludedInMessage()
        {
            var target = new DelegateActivator((c, p) => null, typeof(string));
            try
            {
                target.ActivateInstance(Container.Empty, Enumerable.Empty<Parameter>());
                Assert.Fail("Expected exception not thrown.");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof(DependencyResolutionException), ex);
                Assert.That(ex.Message.Contains(typeof(string).ToString()));
            }
        }
	}
}
