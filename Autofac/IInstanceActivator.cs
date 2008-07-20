using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public interface IInstanceActivator
    {
        object ActivateInstance(ILifetimeScope activationScope, IEnumerable<Parameter> parameters);

        void CompleteActivation(object newInstance, INestedLifetimeScope activationScope);

        event EventHandler Preparing;

        event EventHandler Activating;

        event EventHandler Activated;

        Type BestGuessImplementationType { get; }
    }
}
