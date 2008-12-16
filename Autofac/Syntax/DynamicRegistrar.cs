using Autofac.RegistrationSources;
using Autofac.Disposal;
using Autofac.Lifetime;
using System;
using Autofac.Services;
using Autofac.Events;

namespace Autofac.Syntax
{
    public class DynamicRegistrar : RegistrationData, IDynamicRegistrar, IRegistrationData
    {
        public DynamicRegistrar()
        {
        }

        public IDynamicRegistrar ExternallyOwned()
        {
            Ownership = InstanceOwnership.ExternallyOwned;
            return this;
        }

        public IDynamicRegistrar OwnedByLifetimeScope()
        {
            Ownership = InstanceOwnership.OwnedByLifetimeScope;
            return this;
        }

        public IDynamicRegistrar UnsharedInstances()
        {
            Sharing = InstanceSharing.None;
            Lifetime = new CurrentScopeLifetime();
            return this;
        }

        public IDynamicRegistrar SingleInstance()
        {
            Sharing = InstanceSharing.Shared;
            Lifetime = new RootScopeLifetime();
            return this;
        }

        public IDynamicRegistrar InstancePerLifetimeScope()
        {
            Sharing = InstanceSharing.Shared;
            Lifetime = new CurrentScopeLifetime();
            return this;
        }

        public IDynamicRegistrar InstancePer(object lifetimeScopeTag)
        {
            Sharing = InstanceSharing.None;
            Lifetime = new MatchingScopeLifetime(scope => scope.Tag == lifetimeScopeTag);
            return this;
        }

        public IDynamicRegistrar As(params Service[] services)
        {
            return this;
        }

        public IDynamicRegistrar As(params Type[] services)
        {
            return this;
        }

        public IDynamicRegistrar OnActivating(Action<ActivatingEventArgs<object>> e)
        {
            return this;
        }

        public IDynamicRegistrar OnActivated(Action<ActivatedEventArgs<object>> e)
        {
            return this;
        }

        public IDynamicRegistrar PropertiesAutowired()
        {
            return this;
        }

        public IDynamicRegistrar PropertiesAutowired(bool allowCircularDependencies)
        {
            return this;
        }
    }
}
