using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public interface ILifetimeScope : IDisposable, IComponentContext
    {
        ILifetimeScope BeginLifetimeScope();
    }
}
