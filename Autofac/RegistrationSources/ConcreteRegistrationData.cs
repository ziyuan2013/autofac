using System;
using System.Collections.Generic;
using Autofac.Activators;

namespace Autofac.RegistrationSources
{
    public class ConcreteRegistrationData : RegistrationData, IRegistrationData, IConcreteRegistrationData
    {
        IInstanceActivator _activator;

        public ConcreteRegistrationData(IInstanceActivator activator)
        {
            Id = Guid.NewGuid();
            Activator = Enforce.ArgumentNotNull(activator, "activator");
        }

        public virtual Guid Id { get; set; }

        public virtual IInstanceActivator Activator
        {
            get { return _activator; }
            set { _activator = Enforce.ArgumentNotNull(value, "value"); }
        }
    }
}
