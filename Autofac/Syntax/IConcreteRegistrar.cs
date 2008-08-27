
namespace Autofac.Syntax
{
    public interface IConcreteRegistrar<T>
    {
        IConcreteRegistrar<T> ExternallyOwned();
        IConcreteRegistrar<T> OwnedByLifetimeScope();
        IConcreteRegistrar<T> UnsharedInstances();
        IConcreteRegistrar<T> SingleInstance();
        IConcreteRegistrar<T> InstancePer<TTag>(TTag lifetimeScopeTag);
    }
}
