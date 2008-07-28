using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.Syntax
{
    public interface IConcreteRegistrar<T>
    {
        IConcreteRegistrar<T> ExternallyOwned();
        IConcreteRegistrar<T> OwnedByLifetimeScope();
    }
}
