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

        public bool TryGetDefaultRegistration(Service service, out IComponentRegistration registration)
        {
            throw new NotImplementedException();
        }

        public bool IsRegistered(Service service)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ComponentRegisteredEventArgs> Registered;

        public bool TryGetRegistration(Service service, out IComponentRegistration registration)
        {
            throw new NotImplementedException();
        }

        public void Register(IComponentRegistration registration, bool allowOverrides)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IComponentRegistration> GetRegistrationsProviding(Service service)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IComponentRegistration> Registrations
        {
            get { throw new NotImplementedException(); }
        }

        public void AddRegistrationSource(IRegistrationSource source)
        {
            throw new NotImplementedException();
        }

        public void AddDynamicRegistrationSource(IRegistrationSource source)
        {
            throw new NotImplementedException();
        }
    }
}
