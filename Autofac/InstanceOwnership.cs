using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public enum InstanceOwnership
    {
        OwnedByLifetimeScope,
        ExternallyOwned
    }
}
