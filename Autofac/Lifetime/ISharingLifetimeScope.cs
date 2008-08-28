using System;

namespace Autofac.Lifetime
{
    public interface ISharingLifetimeScope : ILifetimeScope
    {
        ISharingLifetimeScope RootLifetimeScope { get; }

        ISharingLifetimeScope ParentLifetimeScope { get; }

        bool TryGetSharedInstance(Guid id, out object result);

        void AddSharedInstance(Guid id, object newInstance);
    }
}
