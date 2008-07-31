using System;
using System.Collections.Generic;

namespace Autofac
{
    public interface IComponentRegistration
    {
        Guid Id { get; }

        IInstanceActivator Activator { get; }

        IComponentLifetime Lifetime { get; }

        InstanceSharing Sharing { get; }

        InstanceOwnership Ownership { get; }

        IEnumerable<Service> Services { get; }

        IDictionary<string, object> ExtendedProperties { get; }
    }
}
