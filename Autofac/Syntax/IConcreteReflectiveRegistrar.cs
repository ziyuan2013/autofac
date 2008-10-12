using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Services;
using Autofac.Events;
using System.Reflection;
using Autofac.Activators;

namespace Autofac.Syntax
{
    public interface IConcreteReflectiveRegistrar<T>
    {
        IConcreteReflectiveRegistrar<T> ExternallyOwned();
        IConcreteReflectiveRegistrar<T> OwnedByLifetimeScope();

        IConcreteReflectiveRegistrar<T> UnsharedInstances();
        IConcreteReflectiveRegistrar<T> SingleInstance();
        IConcreteReflectiveRegistrar<T> InstancePerLifetimeScope();
        IConcreteReflectiveRegistrar<T> InstancePer(object lifetimeScopeTag);

        IConcreteReflectiveRegistrar<T> As<TService>();
        IConcreteReflectiveRegistrar<T> As<TService1, TService2>();
        IConcreteReflectiveRegistrar<T> As<TService1, TService2, TService3>();
        IConcreteReflectiveRegistrar<T> As(params Service[] services);
        IConcreteReflectiveRegistrar<T> Named(string name);

        IConcreteReflectiveRegistrar<T> PropertiesAutowired();
        IConcreteReflectiveRegistrar<T> PropertiesAutowired(bool allowCircularDependencies);

        IConcreteReflectiveRegistrar<T> OnActivating(Action<ActivatingEventArgs<T>> e);
        IConcreteReflectiveRegistrar<T> OnActivated(Action<ActivatedEventArgs<T>> e);

        IConcreteReflectiveRegistrar<T> FindConstructorsWith(BindingFlags bindingFlags);
        IConcreteReflectiveRegistrar<T> FindConstructorsWith(IConstructorFinder constructorFinder);

        IConcreteReflectiveRegistrar<T> UsingConstructor(params Type[] signature);
        IConcreteReflectiveRegistrar<T> UsingConstructorSelector(IConstructorSelector constructorSelector);
    }
}
