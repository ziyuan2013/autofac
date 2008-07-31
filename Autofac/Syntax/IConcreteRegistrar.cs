
namespace Autofac.Syntax
{
    public interface IConcreteRegistrar<T>
    {
        IConcreteRegistrar<T> ExternallyOwned();
        IConcreteRegistrar<T> OwnedByLifetimeScope();
    }
}
