using System.Collections.Generic;
using Autofac.Lifetime;
using Autofac.Services;
using Autofac.Disposal;

namespace Autofac.RegistrationSources
{
    public class RegistrationData : IRegistrationData
    {
        public RegistrationData()
        {
            Ownership = InstanceOwnership.OwnedByLifetimeScope;
            Lifetime = new RootScopeLifetime();
            Sharing = InstanceSharing.Shared;
            Services = new HashSet<Service>();
            ExtendedProperties = new Dictionary<string, object>();
        }

        public virtual ICollection<Service> Services { get; protected set; }

        public virtual InstanceOwnership Ownership { get; protected set; }

        public virtual IComponentLifetime Lifetime { get; protected set; }

        public virtual InstanceSharing Sharing { get; protected set; }

        public virtual IDictionary<string, object> ExtendedProperties { get; protected set; }
    }
}
