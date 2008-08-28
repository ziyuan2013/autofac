
using System.Collections.Generic;
using Autofac.Services;
namespace Autofac.Syntax
{
    public interface IConcreteRegistrar<T>
    {
        IConcreteRegistrar<T> ExternallyOwned();
        IConcreteRegistrar<T> OwnedByLifetimeScope();

        IConcreteRegistrar<T> UnsharedInstances();
        IConcreteRegistrar<T> SingleInstance();
        IConcreteRegistrar<T> InstancePerLifetimeScope();
        IConcreteRegistrar<T> InstancePer(object lifetimeScopeTag);

        IConcreteRegistrar<T> As<TService>();
        IConcreteRegistrar<T> As<TService1, TService2>();
        IConcreteRegistrar<T> As<TService1, TService2, TService3>();
        IConcreteRegistrar<T> As(params Service[] services);
        IConcreteRegistrar<T> Named(string name);
    }
}
