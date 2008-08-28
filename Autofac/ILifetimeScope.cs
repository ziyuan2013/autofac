using System;
using Autofac.Disposal;

namespace Autofac
{
    public interface ILifetimeScope : IDisposable, IComponentContext
    {
        ILifetimeScope BeginLifetimeScope();

        IDisposer Disposer { get; }

        object Tag { get; set; }
    }
}
