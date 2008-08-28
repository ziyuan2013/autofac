using System;

namespace Autofac.Disposal
{
    public interface IDisposer : IDisposable
    {
        void Add(IDisposable disposable);
    }
}
