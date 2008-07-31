using System;

namespace Autofac
{
    public interface ILifetimeScope : IDisposable, IComponentContext
    {
        ILifetimeScope BeginLifetimeScope();

        IDisposer Disposer { get; }
    }
}
