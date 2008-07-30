using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.RegistrationSources
{
    public class RegistrationData : IRegistrationData
    {
        public virtual ICollection<Service> Services { get; protected set; }

        public virtual InstanceOwnership Ownership { get; protected set; }

        public virtual IComponentLifetime Lifetime { get; protected set; }

        public virtual InstanceSharing Sharing { get; protected set; }

        public virtual IDictionary<string, object> ExtendedProperties { get; protected set; }
    }
}
