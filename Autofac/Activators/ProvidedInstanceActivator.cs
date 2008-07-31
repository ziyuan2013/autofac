using System;
using System.Collections.Generic;

namespace Autofac.Activators
{
    public class ProvidedInstanceActivator : IInstanceActivator
    {
        object _instance;
        bool _activated;

        public ProvidedInstanceActivator(object instance)
        {
            _instance = Enforce.ArgumentNotNull(instance, "instance");
        }

        public object ActivateInstance(ILifetimeScope activationScope, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(activationScope, "activationScope");
            Enforce.ArgumentNotNull(parameters, "parameters");

            if (_activated)
                throw new InvalidOperationException("Already activated the only provided instance");

            _activated = true;

            return _instance;
        }

        public void CompleteActivation(object newInstance, INestedLifetimeScope activationScope)
        {
        }

        public event EventHandler Preparing;

        public event EventHandler Activating;

        public event EventHandler Activated;

        public Type BestGuessImplementationType
        {
            get { return _instance.GetType(); }
        }
    }
}
