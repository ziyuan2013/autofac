using Autofac.Services;
using System;
using Autofac.Events;

namespace Autofac.Syntax
{
    public interface IDynamicRegistrar
    {
        IDynamicRegistrar ExternallyOwned();
        IDynamicRegistrar OwnedByLifetimeScope();

        IDynamicRegistrar UnsharedInstances();
        IDynamicRegistrar SingleInstance();
        IDynamicRegistrar InstancePerLifetimeScope();
        IDynamicRegistrar InstancePer(object lifetimeScopeTag);

        IDynamicRegistrar As(params Service[] services);
        IDynamicRegistrar As(params Type[] services);

        IDynamicRegistrar OnActivating(Action<ActivatingEventArgs<object>> e);
        IDynamicRegistrar OnActivated(Action<ActivatedEventArgs<object>> e);

        IDynamicRegistrar PropertiesAutowired();
        IDynamicRegistrar PropertiesAutowired(bool allowCircularDependencies);
    }
}
