using Autofac.Registry;

namespace Autofac.RegistrationSources
{
    public interface IDeferredRegistrationSource
    {
        IComponentRegistration GetRegistration();
    }
}
