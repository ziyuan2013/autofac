using System.Collections.Generic;
using Autofac.Disposal;
using Autofac.Lifetime;
using Autofac.Services;
using System;
using Autofac.Events;

namespace Autofac.RegistrationSources
{
    public interface IRegistrationData
    {
        ICollection<Service> Services { get; }

        InstanceOwnership Ownership { get; }

        IComponentLifetime Lifetime { get; }

        InstanceSharing Sharing { get; }

        IDictionary<string, object> ExtendedProperties { get; }

        ICollection<EventHandler<ActivatingEventArgs<object>>> ActivatingHandlers { get; }

        ICollection<EventHandler<ActivatedEventArgs<object>>> ActivatedHandlers { get; }
    }
}
