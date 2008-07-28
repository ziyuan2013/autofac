using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Internal;
using Autofac.RegistrationSources;

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
        {
            _defaultService = Enforce.ArgumentNotNull(defaultService, "defaultService");
            Id = Guid.NewGuid();
            Activator = Enforce.ArgumentNotNull(activator, "activator");
            Ownership = InstanceOwnership.OwnedByLifetimeScope;
            Lifetime = new RootScopeLifetime();
            Sharing = InstanceSharing.Shared;
            Services = new HashSet<Service>();
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
    }
}
