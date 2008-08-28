using Autofac.RegistrationSources;
using Autofac.Disposal;

namespace Autofac.Syntax
{
    public class Registrar<T> : RegistrationData, IRegistrar<T>, IRegistrationData
    {
        public Registrar()
        {
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
