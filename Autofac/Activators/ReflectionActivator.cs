using System;
using System.Collections.Generic;

namespace Autofac.Activators
{
    public class ReflectionActivator : InstanceActivator, IInstanceActivator
    {
        Type _implementationType;

        public ReflectionActivator(Type implementationType)
            : base(Enforce.ArgumentNotNull(implementationType, "implementationType"))
        {
            _implementationType = implementationType;
        }

        public object ActivateInstance(ILifetimeScope activationScope, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(activationScope, "activationScope");
            Enforce.ArgumentNotNull(parameters, "parameters");

            return Activator.CreateInstance(_implementationType);
        }
    }
}
