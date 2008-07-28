using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.Activators
{
    public class ReflectionActivator : IInstanceActivator
    {
        public ReflectionActivator(Type implementationType)
        {
        }

        public object ActivateInstance(ILifetimeScope activationScope, IEnumerable<Parameter> parameters)
        {
            throw new NotImplementedException();
        }

        public void CompleteActivation(object newInstance, INestedLifetimeScope activationScope)
        {
            throw new NotImplementedException();
        }

        public event EventHandler Preparing;

        public event EventHandler Activating;

        public event EventHandler Activated;

        public Type BestGuessImplementationType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
