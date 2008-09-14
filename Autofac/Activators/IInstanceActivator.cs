using System;
using System.Collections.Generic;
using Autofac.Lifetime;
using Autofac.Events;

namespace Autofac.Activators
{
    public interface IInstanceActivator
    {
        // The context parameter here should probably be ILifetimeScope in order to reveal Disposer,
        // but will wait until implementing a concrete use case to make the decision
        object ActivateInstance(IComponentContext context, IEnumerable<Parameter> parameters);

        Type BestGuessImplementationType { get; }
    }
}
