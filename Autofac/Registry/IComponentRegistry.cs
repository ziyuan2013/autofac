using System;
using System.Collections.Generic;
using Autofac.RegistrationSources;
using Autofac.Services;

namespace Autofac.Registry
{
    public interface IComponentRegistry : IDisposable
    {
        bool TryGetRegistration(Service service, out IComponentRegistration registration);

        bool IsRegistered(Service service);

        void Register(IComponentRegistration registration);

        IEnumerable<IComponentRegistration> Registrations { get; }

        /// <summary>
        /// Selects from the available registrations after ensuring that any
        /// dynamic registration sources that may provide <paramref name="service"/>
        /// have been invoked.
        /// </summary>
        /// <param name="service">The service for which registrations are sought.</param>
        /// <returns>Registrations supporting <paramref name="service"/>.</returns>
        IEnumerable<IComponentRegistration> RegistrationsFor(Service service);

        event EventHandler<ComponentRegisteredEventArgs> Registered;

        void AddDeferredRegistrationSource(IDeferredRegistrationSource source);

        void AddDynamicRegistrationSource(IDynamicRegistrationSource source);
    }
}
