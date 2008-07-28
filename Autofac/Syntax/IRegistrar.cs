using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.Syntax
{
    public interface IRegistrar<T>
    {
        IRegistrar<T> ExternallyOwned();
        IRegistrar<T> OwnedByLifetimeScope();
    }
}
