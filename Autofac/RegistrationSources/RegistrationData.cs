using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.RegistrationSources
{
    public class RegistrationData : IRegistrationData
    {
        public ICollection<Service> Services { get; protected set; }

        public InstanceOwnership Ownership { get; protected set; }

        public IComponentLifetime Lifetime { get; protected set; }

        public InstanceSharing Sharing { get; protected set; }

        public IDictionary<string, object> ExtendedProperties { get; protected set; }
    }
}
