using System.Collections.Generic;

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
