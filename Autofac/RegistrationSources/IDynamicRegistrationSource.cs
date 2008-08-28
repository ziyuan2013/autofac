using Autofac.Registry;
using Autofac.Services;

namespace Autofac.RegistrationSources
{
    public interface IDynamicRegistrationSource
    {
        bool TryGetRegistration(Service service, out IComponentRegistration registration);
    }
}
