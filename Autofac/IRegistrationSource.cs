using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public interface IRegistrationSource
    {
        bool TryGetRegistration(Service service, out IComponentRegistration registration);
    }
}
