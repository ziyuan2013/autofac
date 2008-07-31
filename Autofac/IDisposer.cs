using System;

namespace Autofac
{
    public interface IDisposer : IDisposable
    {
        void Add(IDisposable disposable);
    }
}
