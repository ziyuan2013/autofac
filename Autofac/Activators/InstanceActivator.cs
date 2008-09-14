using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Lifetime;
using Autofac.Events;

namespace Autofac.Activators
{
    public abstract class InstanceActivator
    {
        Type _bestGuessImplementationType;

        protected InstanceActivator(Type bestGuessImplementationType)
        {
            _bestGuessImplementationType = Enforce.ArgumentNotNull(bestGuessImplementationType, "bestGuessImplementationType");
        }

        public Type BestGuessImplementationType
        {
            get { return _bestGuessImplementationType; }
        }
    }
}
