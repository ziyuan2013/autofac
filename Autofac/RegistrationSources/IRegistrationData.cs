using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.RegistrationSources
{
    public interface IRegistrationData
    {
        ICollection<Service> Services { get; }

        InstanceOwnership Ownership { get; }

        IComponentLifetime Lifetime { get; }

        InstanceSharing Sharing { get; }

        IDictionary<string, object> ExtendedProperties { get; }
    }
}
