using System.Collections.Generic;
using NUnit.Framework;
using Autofac;
using Autofac.Registry;
using Autofac.Services;

namespace Autofac.Tests
{
    [TestFixture]
    public class OpenGenericTests
    {
        [Test]
        public void ClosesOpenGenericType()
        {
            var container = new Container();
            container.RegisterGenericType(typeof(List<>))
                .As(typeof(ICollection<>))
                .UnsharedInstances();

            ICollection<int> coll = container.Resolve<ICollection<int>>();
            ICollection<int> coll2 = container.Resolve<ICollection<int>>();

            Assert.IsNotNull(coll);
            Assert.IsNotNull(coll2);
            Assert.AreNotSame(coll, coll2);
            Assert.IsTrue(coll.GetType().GetGenericTypeDefinition() == typeof(List<>));
        }

        [Test]
        public void GenericTypeRegistrationExposesImplementationType()
        {
            var container = new Container();
            container.RegisterGenericType(typeof(List<>))
                .As(typeof(IEnumerable<>));

            IComponentRegistration cr;
            Assert.IsTrue(container.ComponentRegistry.TryGetRegistration(
                new TypedService(typeof(IEnumerable<int>)), out cr));
            Assert.AreEqual(typeof(List<int>), cr.Activator.BestGuessImplementationType);
        }

        [Test]
        public void ResolvingClosedGenericTypeFiresActivatingEventOnce()
        {
            int activatingFired = 0;
            var container = new Container();
            container.RegisterGenericType(typeof(List<>))
                .As(typeof(IEnumerable<>))
                .UsingConstructor() // necessary to prevent greediness
                .OnActivating(o => ++activatingFired);

            container.Resolve<IEnumerable<int>>();
            Assert.AreEqual(1, activatingFired);
        }
    }
}
