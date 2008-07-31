
namespace Autofac.Syntax
{
    public interface IRegistrar<T>
    {
        IRegistrar<T> ExternallyOwned();
        IRegistrar<T> OwnedByLifetimeScope();
    }
}
