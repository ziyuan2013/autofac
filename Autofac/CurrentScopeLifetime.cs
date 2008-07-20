using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Internal;

namespace Autofac
{
    public class CurrentScopeLifetime : IComponentLifetime
    {
        public INestedLifetimeScope FindScope(INestedLifetimeScope mostNestedVisibleScope)
        {
            Enforce.ArgumentNotNull(mostNestedVisibleScope, "mostNestedVisibleScope");
            return mostNestedVisibleScope;
        }
    }
}
