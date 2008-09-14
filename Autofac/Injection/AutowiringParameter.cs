using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Autofac.Registry;
using Autofac.Services;

namespace Autofac.Injection
{
    public class AutowiringParameter : Parameter
    {
        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            IComponentRegistration registration;
            if (context.ComponentRegistry.TryGetRegistration(new TypedService(pi.ParameterType), out registration))
            {
                valueProvider = () => context.Resolve(registration, Enumerable.Empty<Parameter>());
                return true;
            }
            else
            {
                valueProvider = null;
                return false;
            }
        }
    }
}
