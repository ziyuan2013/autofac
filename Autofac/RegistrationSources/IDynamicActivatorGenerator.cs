using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Services;
using Autofac.Activators;

namespace Autofac.RegistrationSources
{
    public interface IDynamicActivatorGenerator
    {
        bool TryGenerateActivator(Service service, IEnumerable<Service> configuredServices, out IInstanceActivator activator, out IEnumerable<Service> services);
    }
}
