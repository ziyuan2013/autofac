using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac
{
    public class ComponentRegistration : IComponentRegistration
    {
        public ComponentRegistration(
            Guid id,
            IInstanceActivator activator,
            IComponentLifetime lifetime,
            InstanceSharing sharing,
            InstanceOwnership ownership,
            IEnumerable<Service> services,
            IDictionary<string, object> extendedProperties)
        {
            Id = id;
            Activator = Enforce.ArgumentNotNull(activator, "activator");
            Lifetime = Enforce.ArgumentNotNull(lifetime, "lifetime");
            Sharing = sharing;
            Ownership = ownership;
            Services = Enforce.ArgumentNotNull(services, "services").ToList();
            ExtendedProperties = new Dictionary<string, object>(
                Enforce.ArgumentNotNull(extendedProperties, "extendedProperties"));
        }

        public Guid Id { get; private set; }

        public IInstanceActivator Activator { get; private set; }

        public IComponentLifetime Lifetime { get; private set; }

        public InstanceSharing Sharing { get; private set; }

        public InstanceOwnership Ownership { get; private set; }

        public IEnumerable<Service> Services { get; private set; }

        public IDictionary<string, object> ExtendedProperties { get; private set; }
    }
}
