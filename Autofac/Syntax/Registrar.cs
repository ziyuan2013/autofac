using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Internal;
using Autofac.Standard;
using Autofac.RegistrationSources;

namespace Autofac.Syntax
{
    public class Registrar<T> : RegistrationData, IRegistrar<T>, IRegistrationData
    {
        public Registrar()
        {
            Ownership = InstanceOwnership.OwnedByLifetimeScope;
            Lifetime = new RootScopeLifetime();
            Sharing = InstanceSharing.Shared;
            Services = new HashSet<Service>();
        }

        public IRegistrar<T> ExternallyOwned()
        {
            Ownership = InstanceOwnership.ExternallyOwned;
            return this;
        }

        public IRegistrar<T> OwnedByLifetimeScope()
        {
            Ownership = InstanceOwnership.OwnedByLifetimeScope;
            return this;
        }
    }
}
