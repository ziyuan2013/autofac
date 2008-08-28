using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Lifetime;

namespace Autofac.Activators
{
    public abstract class InstanceActivator
    {
        Type _bestGuessImplementationType;

        protected InstanceActivator(Type bestGuessImplementationType)
        {
            _bestGuessImplementationType = Enforce.ArgumentNotNull(bestGuessImplementationType, "bestGuessImplementationType");
        }

        public void CompleteActivation(object newInstance, ISharingLifetimeScope activationScope)
        {
        }

        public event EventHandler Preparing = (s, e) => { };

        public event EventHandler Activating = (s, e) => { };

        public event EventHandler Activated = (s, e) => { };

        public Type BestGuessImplementationType
        {
            get { return _bestGuessImplementationType; }
        }
    }
}
