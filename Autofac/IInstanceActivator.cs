using System;
using System.Collections.Generic;

namespace Autofac
{
    public interface IInstanceActivator
    {
        // The context parameter here should probably be ILifetimeScope in order to reveal Disposer,
        // but will wait until implementing a concrete use case to make the decision
        object ActivateInstance(IComponentContext context, IEnumerable<Parameter> parameters);

        void CompleteActivation(object newInstance, ISharingLifetimeScope activationScope);

        event EventHandler Preparing;

        event EventHandler Activating;

        event EventHandler Activated;

        Type BestGuessImplementationType { get; }
    }
}
