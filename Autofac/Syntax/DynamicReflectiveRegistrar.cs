using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.RegistrationSources;
using Autofac.Services;
using System.Reflection;
using Autofac.Activators;

namespace Autofac.Syntax
{
    public class DynamicReflectiveRegistrar : IDynamicReflectiveRegistrar
    {
        IDynamicRegistrar _registrar;
        IReflectiveActivatorData _activatorGenerator;

        public DynamicReflectiveRegistrar(IDynamicRegistrar registrar, IReflectiveActivatorData activatorData)
        {
            _registrar = Enforce.ArgumentNotNull(registrar, "registrar");
            _activatorGenerator = Enforce.ArgumentNotNull(activatorData, "activatorData");
        }

        public IDynamicReflectiveRegistrar ExternallyOwned()
        {
            _registrar.ExternallyOwned();
            return this;
        }

        public IDynamicReflectiveRegistrar OwnedByLifetimeScope()
        {
            _registrar.OwnedByLifetimeScope();
            return this;
        }

        public IDynamicReflectiveRegistrar UnsharedInstances()
        {
            _registrar.UnsharedInstances();
            return this;
        }

        public IDynamicReflectiveRegistrar SingleInstance()
        {
            _registrar.UnsharedInstances();
            return this;
        }

        public IDynamicReflectiveRegistrar InstancePerLifetimeScope()
        {
            _registrar.InstancePerLifetimeScope();
            return this;
        }

        public IDynamicReflectiveRegistrar InstancePer(object lifetimeScopeTag)
        {
            _registrar.InstancePer(lifetimeScopeTag);
            return this;
        }

        public IDynamicReflectiveRegistrar As(params Service[] services)
        {
            _registrar.As(services);
            return this;
        }

        public IDynamicReflectiveRegistrar As(params Type[] services)
        {
            _registrar.As(services);
            return this;
        }

        public IDynamicReflectiveRegistrar PropertiesAutowired()
        {
            _registrar.PropertiesAutowired();
            return this;
        }

        public IDynamicReflectiveRegistrar PropertiesAutowired(bool allowCircularDependencies)
        {
            _registrar.PropertiesAutowired(allowCircularDependencies);
            return this;
        }

        public IDynamicReflectiveRegistrar OnActivating(Action<Autofac.Events.ActivatingEventArgs<object>> e)
        {
            _registrar.OnActivating(e);
            return this;
        }

        public IDynamicReflectiveRegistrar OnActivated(Action<Autofac.Events.ActivatedEventArgs<object>> e)
        {
            _registrar.OnActivated(e);
            return this;
        }

        public IDynamicReflectiveRegistrar FindConstructorsWith(BindingFlags bindingFlags)
        {
            return FindConstructorsWith(new BindingFlagsConstructorFinder(bindingFlags));
        }

        public IDynamicReflectiveRegistrar FindConstructorsWith(IConstructorFinder constructorFinder)
        {
            _activatorGenerator.SetConstructorFinder(Enforce.ArgumentNotNull(constructorFinder, "constructorFinder"));
            return this;
        }

        public IDynamicReflectiveRegistrar UsingConstructor(params Type[] signature)
        {
            return UsingConstructorSelector(new MatchingSignatureConstructorSelector(signature));
        }

        public IDynamicReflectiveRegistrar UsingConstructorSelector(IConstructorSelector constructorSelector)
        {
            _activatorGenerator.SetConstructorSelector(Enforce.ArgumentNotNull(constructorSelector, "constructorSelector"));
            return this;
        }
    }
}
