using Autofac.RegistrationSources;
using Autofac.Disposal;
using Autofac.Lifetime;
using System;
using Autofac.Services;
using Autofac.Events;
using System.Linq;
using Autofac.Injection;
using System.Collections.Generic;

namespace Autofac.Syntax
{
    public class DynamicRegistrar : RegistrationData, IDynamicRegistrar, IRegistrationData
    {
        Service _defaultService;

        public DynamicRegistrar(Service defaultService)
        {
            _defaultService = Enforce.ArgumentNotNull(defaultService, "defaultService");
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

        public IDynamicRegistrar As<TService>()
        {
            return As(new TypedService(typeof(TService)));
        }

        public IDynamicRegistrar As<TService1, TService2>()
        {
            return As(new TypedService(typeof(TService1)), new TypedService(typeof(TService2)));
        }

        public IDynamicRegistrar As<TService1, TService2, TService3>()
        {
            return As(new TypedService(typeof(TService1)), new TypedService(typeof(TService2)), new TypedService(typeof(TService3)));
        }

        public IDynamicRegistrar As(params Type[] services)
        {
            return As(services.Select(t => new TypedService(t)).Cast<Service>().ToArray());
        }

        public IDynamicRegistrar As(params Service[] services)
        {
            Enforce.ArgumentNotNull(services, "services");

            foreach (var service in services)
                base.Services.Add(service);

            return this;
        }

        public IDynamicRegistrar OnActivating(Action<ActivatingEventArgs<object>> handler)
        {
            Enforce.ArgumentNotNull(handler, "handler");
            ActivatingHandlers.Add((s, e) =>
            {
                handler(new ActivatingEventArgs<object>(e.Context, e.Component, e.Instance));
            });
            return this;
        }

        public IDynamicRegistrar OnActivated(Action<ActivatedEventArgs<object>> handler)
        {
            Enforce.ArgumentNotNull(handler, "handler");
            ActivatedHandlers.Add((s, e) =>
            {
                handler(new ActivatedEventArgs<object>(e.Context, e.Component, e.Instance));
            });
            return this;
        }

        public IDynamicRegistrar PropertiesAutowired()
        {
            var injector = new AutowiringPropertyInjector();
            ActivatingHandlers.Add((s, e) =>
            {
                injector.InjectProperties(e.Context, e.Instance, true);
            });
            return this;
        }

        public IDynamicRegistrar PropertiesAutowired(bool allowCircularDependencies)
        {
            if (allowCircularDependencies)
            {
                var injector = new AutowiringPropertyInjector();
                ActivatedHandlers.Add((s, e) =>
                {
                    injector.InjectProperties(e.Context, e.Instance, true);
                });
                return this;
            }
            else
            {
                return PropertiesAutowired();
            }
        }

        public override ICollection<Service> Services
        {
            get
            {
                var result = base.Services;
                if (!result.Any())
                    result = new[] { _defaultService };
                return result;
            }
            protected set
            {
                base.Services = value;
            }
        }
    }
}
