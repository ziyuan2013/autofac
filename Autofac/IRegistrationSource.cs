
namespace Autofac
{
    public interface IRegistrationSource
    {
        bool TryGetRegistration(Service service, out IComponentRegistration registration);
    }
}
