using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Autofac.Activators;
using Autofac.Events;
using Autofac.Services;

namespace Autofac.Syntax
{
    public interface IDynamicReflectiveRegistrar
    {
        IDynamicReflectiveRegistrar ExternallyOwned();
        IDynamicReflectiveRegistrar OwnedByLifetimeScope();

        IDynamicReflectiveRegistrar UnsharedInstances();
        IDynamicReflectiveRegistrar SingleInstance();
        IDynamicReflectiveRegistrar InstancePerLifetimeScope();
        IDynamicReflectiveRegistrar InstancePer(object lifetimeScopeTag);

        IDynamicReflectiveRegistrar As(params Service[] services);
        IDynamicReflectiveRegistrar As(params Type[] services);

        IDynamicReflectiveRegistrar OnActivating(Action<ActivatingEventArgs<object>> e);
        IDynamicReflectiveRegistrar OnActivated(Action<ActivatedEventArgs<object>> e);

        IDynamicReflectiveRegistrar PropertiesAutowired();
        IDynamicReflectiveRegistrar PropertiesAutowired(bool allowCircularDependencies);

        IDynamicReflectiveRegistrar FindConstructorsWith(BindingFlags bindingFlags);
        IDynamicReflectiveRegistrar FindConstructorsWith(IConstructorFinder constructorFinder);

        IDynamicReflectiveRegistrar UsingConstructor(params Type[] signature);
        IDynamicReflectiveRegistrar UsingConstructorSelector(IConstructorSelector constructorSelector);
    }
}
