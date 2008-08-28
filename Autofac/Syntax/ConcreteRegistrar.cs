using System.Collections.Generic;
using System.Linq;
using Autofac.RegistrationSources;
using Autofac.Lifetime;
using Autofac.Activators;
using Autofac.Services;
using Autofac.Disposal;

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
    }
}
