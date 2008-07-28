using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.RegistrationSources
{
    public interface IConcreteRegistrationData : IRegistrationData
    {
        Guid Id { get; }
        IInstanceActivator Activator { get; }
    }
}
