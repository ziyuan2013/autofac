using System;
using System.Collections.Generic;

namespace Autofac.RegistrationSources
{
    public class ConcreteRegistrationData : RegistrationData, IRegistrationData, IConcreteRegistrationData
    {
        public ConcreteRegistrationData(IInstanceActivator activator)
        {
            Id = Guid.NewGuid();
            Activator = Enforce.ArgumentNotNull(activator, "activator");
            Ownership = InstanceOwnership.OwnedByLifetimeScope;
            Lifetime = new RootScopeLifetime();
            Sharing = InstanceSharing.Shared;
            Services = new HashSet<Service>();
        }

        public virtual Guid Id { get; protected set; }

        public virtual IInstanceActivator Activator { get; protected set; }
    }
}
