using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Internal;

namespace Autofac
{
    public class ComponentRegistry : Disposable, IComponentRegistry
    {
        ICollection<Service> _unregisteredServices = new HashSet<Service>();
        ICollection<IRegistrationSource>
            _registrationSources = new List<IRegistrationSource>(),
            _dynamicRegistrationSources = new List<IRegistrationSource>();
        IDictionary<Service, IComponentRegistration> _defaultRegistrations = new Dictionary<Service, IComponentRegistration>();

        public ComponentRegistry()
        {
            Registrations = new List<IComponentRegistration>();
        }

        public bool IsRegistered(Service service)
        {
            Enforce.ArgumentNotNull(service, "service");

            IComponentRegistration unused;
            return TryGetRegistration(service, out unused);
        }

        public event EventHandler<ComponentRegisteredEventArgs> Registered;

        public bool TryGetRegistration(Service service, out IComponentRegistration registration)
        {
            Enforce.ArgumentNotNull(service, "service");

            if (_defaultRegistrations.TryGetValue(service, out registration))
                return true;

            if (_unregisteredServices.Contains(service))
                return false;

            foreach (var rs in _registrationSources)
            {
                if (rs.TryGetRegistration(service, out registration))
                {
                    _registrationSources.Remove(rs);
                    Register(registration, false);
                    return true;
                }
            }

            foreach (var rs in _dynamicRegistrationSources)
            {
                if (rs.TryGetRegistration(service, out registration))
                {
                    Register(registration, false);
                    return true;
                }
            }

            _unregisteredServices.Add(service);
            return false;
        }

        public void Register(IComponentRegistration registration, bool allowOverrides)
        {
            Enforce.ArgumentNotNull(registration, "registration");
            foreach (var service in registration.Services)
                _defaultRegistrations.Add(service, registration);
        }

        public IEnumerable<IComponentRegistration> GetRegistrationsProviding(Service service)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IComponentRegistration> Registrations { get; private set; }

        public void AddRegistrationSource(IRegistrationSource source)
        {
            _registrationSources.Add(Enforce.ArgumentNotNull(source, "source"));
            RegistrationSourcesChanged();
        }

        public void AddDynamicRegistrationSource(IRegistrationSource source)
        {
            _dynamicRegistrationSources.Add(Enforce.ArgumentNotNull(source, "source"));
            RegistrationSourcesChanged();
        }

        void RegistrationSourcesChanged()
        {
            _unregisteredServices.Clear();
        }
    }
}
