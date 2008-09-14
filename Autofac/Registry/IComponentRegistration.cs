using System;
using System.Collections.Generic;
using Autofac.Lifetime;
using Autofac.Disposal;
using Autofac.Services;
using Autofac.Activators;
using Autofac.Events;

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

        event EventHandler<ActivatingEventArgs<object>> Activating;

        // Temporary :)
        void RaiseActivating(IComponentContext activationContext, object instance);

        event EventHandler<ActivatedEventArgs<object>> Activated;

        void RaiseActivated(IComponentContext activationContext, object instance);
    }
}
