using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.RegistrationSources;
using Autofac.Services;
using Autofac.Registry;
using Autofac.Activators;
using Autofac.Lifetime;
using Autofac.Disposal;

namespace Autofac.OwnedInstances
{
    public class OwnedRegistrationSource : IDynamicRegistrationSource
    {
        public bool TryGetRegistration(Service service, Func<Service, bool> registeredServicesTest, out IComponentRegistration registration)
        {
            Enforce.ArgumentNotNull(service, "service");

            var ts = service as TypedService;
            if (ts != null &&
                ts.ServiceType.IsGenericType &&
                ts.ServiceType.GetGenericTypeDefinition() == typeof(Owned<>))
            {
                var ownedInstanceType = ts.ServiceType.GetGenericArguments()[0];

                if (registeredServicesTest(new TypedService(ownedInstanceType)))
                {
                    registration = new ComponentRegistration(
                        Guid.NewGuid(),
                        new DelegateActivator(ts.ServiceType, (c, p) =>
                        {
                            var lifetime = c.Resolve<ILifetimeScope>().BeginLifetimeScope();
                            try
                            {
                                var value = lifetime.Resolve(ownedInstanceType, p);
                                return Activator.CreateInstance(ts.ServiceType, new object[] { value, lifetime });
                            }
                            catch
                            {
                                lifetime.Dispose();
                                throw;
                            }
                        }),
                        new CurrentScopeLifetime(),
                        InstanceSharing.None,
                        InstanceOwnership.ExternallyOwned,
                        new Service[] { new TypedService(ts.ServiceType) },
                        new Dictionary<string, object>());

                    return true;
                }
            }

            registration = null;
            return false;
        }
    }
}
