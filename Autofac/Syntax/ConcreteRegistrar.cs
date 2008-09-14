using System.Collections.Generic;
using System.Linq;
using Autofac.RegistrationSources;
using Autofac.Lifetime;
using Autofac.Activators;
using Autofac.Services;
using Autofac.Disposal;
using Autofac.Events;
using System;
using Autofac.Injection;

namespace Autofac.Syntax
{
    public class ConcreteRegistrar<T> : ConcreteRegistrationData, IConcreteRegistrationData, IConcreteRegistrar<T>
    {
        Service _defaultService;

        public ConcreteRegistrar(IInstanceActivator activator)
            : this(activator, new TypedService(Enforce.ArgumentNotNull(activator, "activator").BestGuessImplementationType))
        {
        }

        public ConcreteRegistrar(IInstanceActivator activator, Service defaultService)
            : base(activator)
        {
            _defaultService = Enforce.ArgumentNotNull(defaultService, "defaultService");
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

        public IConcreteRegistrar<T> ExternallyOwned()
        {
            Ownership = InstanceOwnership.ExternallyOwned;
            return this;
        }

        public IConcreteRegistrar<T> OwnedByLifetimeScope()
        {
            Ownership = InstanceOwnership.OwnedByLifetimeScope;
            return this;
        }

        public IConcreteRegistrar<T> UnsharedInstances()
        {
            Sharing = InstanceSharing.None;
            Lifetime = new CurrentScopeLifetime();
            return this;
        }

        public IConcreteRegistrar<T> SingleInstance()
        {
            Sharing = InstanceSharing.Shared;
            Lifetime = new RootScopeLifetime();
            return this;
        }

        public IConcreteRegistrar<T> InstancePerLifetimeScope()
        {
            Sharing = InstanceSharing.Shared;
            Lifetime = new CurrentScopeLifetime();
            return this;
        }

        public IConcreteRegistrar<T> InstancePer(object lifetimeScopeTag)
        {
            Sharing = InstanceSharing.None;
            Lifetime = new MatchingScopeLifetime(scope => scope.Tag == lifetimeScopeTag);
            return this;
        }

        public IConcreteRegistrar<T> As<TService>()
        {
            return As(new TypedService(typeof(TService)));
        }

        public IConcreteRegistrar<T> As<TService1, TService2>()
        {
            return As(new TypedService(typeof(TService1)), new TypedService(typeof(TService2)));
        }

        public IConcreteRegistrar<T> As<TService1, TService2, TService3>()
        {
            return As(new TypedService(typeof(TService1)), new TypedService(typeof(TService2)), new TypedService(typeof(TService3)));
        }

        public IConcreteRegistrar<T> As(params Service[] services)
        {
            Enforce.ArgumentNotNull(services, "services");

            foreach (var service in services)
                base.Services.Add(service);

            return this;
        }

        public IConcreteRegistrar<T> Named(string name)
        {
            return As(new NamedService(name));
        }

        public IConcreteRegistrar<T> OnActivating(Action<ActivatingEventArgs<T>> handler)
        {
            ActivatingHandlers.Add((s, e) =>
            {
                handler(new ActivatingEventArgs<T>(e.Context, e.Component, (T)e.Instance));
            });

            return this;
        }

        public IConcreteRegistrar<T> OnActivated(Action<ActivatedEventArgs<T>> handler)
        {
            ActivatedHandlers.Add((s, e) =>
            {
                handler(new ActivatedEventArgs<T>(e.Context, e.Component, (T)e.Instance));
            });

            return this;
        }

        public IConcreteRegistrar<T> PropertiesAutowired()
        {
            var injector = new AutowiringPropertyInjector();
            ActivatingHandlers.Add((s, e) =>
            {
                injector.InjectProperties(e.Context, e.Instance, true);
            });
            return this;
        }

        public IConcreteRegistrar<T> PropertiesAutowired(bool allowCircularDependencies)
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
    }
}
