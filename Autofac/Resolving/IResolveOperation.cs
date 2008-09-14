using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Registry;
using Autofac.Lifetime;

namespace Autofac.Resolving
{
    interface IResolveOperation
    {
        object Resolve(ISharingLifetimeScope activationScope, IComponentRegistration registration, IEnumerable<Parameter> parameters);
    }
}
