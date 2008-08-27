
namespace Autofac
{
    public interface IDeferredRegistrationSource
    {
        IComponentRegistration GetRegistration();
    }
}
