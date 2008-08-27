
namespace Autofac
{
    public interface IDynamicRegistrationSource
    {
        bool TryGetRegistration(Service service, out IComponentRegistration registration);
    }
}
