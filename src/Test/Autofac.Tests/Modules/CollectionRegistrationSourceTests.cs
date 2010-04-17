using System.Collections.Generic;
using System.Linq;
using Autofac.Builder;
using Autofac.Modules;
using NUnit.Framework;

namespace Autofac.Tests.Modules
{
    [TestFixture]
    public class CollectionRegistrationSourceTests
    {
        [Test]
        public void WhenResolvingCollectionMultipleTimesInHierarchy_EachComponentIncludedOnlyOnce()
        {
            var cb = new ContainerBuilder();
            cb.Register(ctx => new object()).FactoryScoped();
            var c = cb.Build();
            c.AddRegistrationSource(new CollectionRegistrationSource());
            var inner = c.CreateInnerContainer();
            inner.Resolve<IEnumerable<object>>();

            var objects = inner.Resolve<IEnumerable<object>>();
            Assert.AreEqual(1, objects.Count());
        }
    }
}
