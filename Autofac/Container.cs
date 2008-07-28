using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Internal;

namespace Autofac
{
    public class Container : Disposable, IContainer, ILifetimeScope, IDisposable
    {
        IComponentRegistry _componentRegistry;
        ILifetimeScope _rootLifetimeScope;

        public Container()
        {
            _componentRegistry = new ComponentRegistry();
            _rootLifetimeScope = new LifetimeScope(_componentRegistry);
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return _rootLifetimeScope.BeginLifetimeScope();
        }

        public bool TryResolve(Service service, IEnumerable<Parameter> parameters, out object instance)
        {
            return _rootLifetimeScope.TryResolve(service, parameters, out instance);
        }

        public IComponentRegistry ComponentRegistry
        {
            get { return _componentRegistry; }
        }

        public IDisposer Disposer
        {
            get { return _rootLifetimeScope.Disposer; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rootLifetimeScope.Dispose();
                _componentRegistry.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
