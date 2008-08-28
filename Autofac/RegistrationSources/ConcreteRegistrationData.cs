using System;
using System.Collections.Generic;

namespace Autofac.RegistrationSources
{
    public class ConcreteRegistrationData : RegistrationData, IRegistrationData, IConcreteRegistrationData
    {
        public ConcreteRegistrationData(IInstanceActivator activator)
        {
            Id = Guid.NewGuid();
            Activator = Enforce.ArgumentNotNull(activator, "activator");
        }

        public virtual Guid Id { get; protected set; }

        public virtual IInstanceActivator Activator { get; protected set; }
    }
}
