using Autofac.Registry;
using Autofac.Services;
using System;

namespace Autofac.RegistrationSources
{
    public interface IDynamicRegistrationSource
    {
        bool TryGetRegistration(Service service, Func<Service, bool> registeredServicesTest, out IComponentRegistration registration);
    }
}
