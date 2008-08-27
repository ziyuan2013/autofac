using System;
using System.Collections.Generic;

namespace Autofac
{
    public interface IComponentRegistry : IDisposable
    {
        bool TryGetRegistration(Service service, out IComponentRegistration registration);

        bool IsRegistered(Service service);

        void Register(IComponentRegistration registration);

        IEnumerable<IComponentRegistration> GetRegistrationsProviding(Service service);

        IEnumerable<IComponentRegistration> Registrations { get; }

        event EventHandler<ComponentRegisteredEventArgs> Registered;

        void AddDeferredRegistrationSource(IDeferredRegistrationSource source);

        void AddDynamicRegistrationSource(IDynamicRegistrationSource source);
    }
}
