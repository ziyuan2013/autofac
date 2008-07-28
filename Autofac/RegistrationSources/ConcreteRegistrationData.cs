using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.RegistrationSources
{
    public class ConcreteRegistrationData : RegistrationData, IRegistrationData, IConcreteRegistrationData
    {
        public Guid Id { get; protected set; }

        public IInstanceActivator Activator { get; protected set;  }
    }
}
