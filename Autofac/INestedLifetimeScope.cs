using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public interface INestedLifetimeScope : ILifetimeScope
    {
        INestedLifetimeScope RootLifetimeScope { get; }

        INestedLifetimeScope ParentLifetimeScope { get; }

        bool TryGetSharedInstance(Guid id, out object result);

        void AddSharedInstance(Guid id, object newInstance);
    }
}
