using System.Collections.Generic;
using Autofac.Disposal;
using Autofac.Lifetime;
using Autofac.Services;

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
