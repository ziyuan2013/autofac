using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Autofac.Registry;
using Autofac.Services;
using Autofac.RegistrationSources;
using Autofac.Activators;
using Autofac.Lifetime;
using Autofac.Disposal;

namespace Autofac.Tests
{
    [TestFixture]
    public class ComponentRegistryTests
    {
        class ObjectRegistrationSource : IDynamicRegistrationSource
        {
            public bool TryGetRegistration(Service service, Func<Service, bool> registeredServicesTest, out IComponentRegistration registration)
            {
                var objectService = new TypedService(typeof(object));
                if (service == objectService)
                {
                    registration = new ComponentRegistration(
                        Guid.NewGuid(),
                        new ProvidedInstanceActivator(new object()),
                        new RootScopeLifetime(),
                        InstanceSharing.Shared,
                        InstanceOwnership.OwnedByLifetimeScope,
                        new Service[] { objectService },
                        new Dictionary<string, object>());
                    return true;
                }
                else
                {
                    registration = null;
                    return false;
                }
            }
        }

        [Test]
        public void RegistrationsForServiceIncludeDynamicSources()
        {
            var registry = new ComponentRegistry();
            registry.AddDynamicRegistrationSource(new ObjectRegistrationSource());
            Assert.IsFalse(registry.Registrations.Where(
                r => r.Services.Contains(new TypedService(typeof(object)))).Any());
            Assert.IsTrue(registry.RegistrationsFor(new TypedService(typeof(object))).Count() == 1);
        }
    }
}
