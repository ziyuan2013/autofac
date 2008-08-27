using System;
using System.Collections.Generic;
using System.Linq;

namespace Autofac
{
    class ComponentRegistry : Disposable, IComponentRegistry
    {
        ICollection<Service> _unregisteredServices = new HashSet<Service>();
        ICollection<IDeferredRegistrationSource> _registrationSources = new List<IDeferredRegistrationSource>();
        ICollection<IDynamicRegistrationSource> _dynamicRegistrationSources = new List<IDynamicRegistrationSource>();
        IDictionary<Service, IComponentRegistration> _defaultRegistrations = new Dictionary<Service, IComponentRegistration>();
        ICollection<IComponentRegistration> _registrations = new List<IComponentRegistration>();

        public ComponentRegistry()
        {
        }

        public bool IsRegistered(Service service)
        {
            Enforce.ArgumentNotNull(service, "service");

            IComponentRegistration unused;
            return TryGetRegistration(service, out unused);
        }

        public event EventHandler<ComponentRegisteredEventArgs> Registered = (s, e) => { };

        public bool TryGetRegistration(Service service, out IComponentRegistration registration)
        {
            Enforce.ArgumentNotNull(service, "service");

            DeferredInitialise();

            if (_defaultRegistrations.TryGetValue(service, out registration))
                return true;

            if (_unregisteredServices.Contains(service))
                return false;

            foreach (var rs in _dynamicRegistrationSources)
            {
                if (rs.TryGetRegistration(service, out registration))
                {
                    Register(registration);
                    return true;
                }
            }

            _unregisteredServices.Add(service);
            return false;
        }

        private void DeferredInitialise()
        {
            var sources = _registrationSources;
            _registrationSources = new List<IDeferredRegistrationSource>();
            foreach (var rs in sources)
            {
                Register(rs.GetRegistration());
            }
        }

        public void Register(IComponentRegistration registration)
        {
            Enforce.ArgumentNotNull(registration, "registration");
            foreach (var service in registration.Services)
            {
                _defaultRegistrations[service] = registration;
                _unregisteredServices.Remove(service);
            }
        }

        public IEnumerable<IComponentRegistration> GetRegistrationsProviding(Service service)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IComponentRegistration> Registrations
        {
            get
            {
                DeferredInitialise();
                return _registrations.ToList();
            }
        }

        public void AddDeferredRegistrationSource(IDeferredRegistrationSource source)
        {
            _registrationSources.Add(Enforce.ArgumentNotNull(source, "source"));
        }

        public void AddDynamicRegistrationSource(IDynamicRegistrationSource source)
        {
            _dynamicRegistrationSources.Add(Enforce.ArgumentNotNull(source, "source"));
            _unregisteredServices.Clear();
        }
    }
}
