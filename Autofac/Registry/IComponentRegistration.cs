using System;
using System.Collections.Generic;
using Autofac.Lifetime;
using Autofac.Disposal;
using Autofac.Services;
using Autofac.Activators;

namespace Autofac.Registry
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
