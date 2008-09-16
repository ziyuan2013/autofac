using System;
using System.Collections.Generic;
using Autofac.Registry;
using Autofac.Disposal;
using Autofac.Services;
using Autofac.Lifetime;
using Autofac.SelfRegistration;
using Autofac.Activators;

namespace Autofac
{
    public class Container : Disposable, IContainer, ILifetimeScope, IDisposable
    {
        IComponentRegistry _componentRegistry;
        ILifetimeScope _rootLifetimeScope;

        public Container()
        {
            _componentRegistry = new ComponentRegistry();

            // Lots of ugly cruft around self-registration, needs some refactoring but
            // not sure of the right approach yet

            _componentRegistry.Register(new ComponentRegistration(
                Guid.NewGuid(),
                new DelegateActivator(typeof(IndirectReference<ILifetimeScope>), (c, p) => new IndirectReference<ILifetimeScope>()),
                new CurrentScopeLifetime(),
                InstanceSharing.Shared,
                InstanceOwnership.ExternallyOwned,
                new Service[] { new TypedService(typeof(IndirectReference<ILifetimeScope>)) },
                new Dictionary<string, object>()));

            _componentRegistry.Register(new ComponentRegistration(
                Guid.NewGuid(),
                new DelegateActivator(typeof(LifetimeScope), (c, p) => c.Resolve<IndirectReference<ILifetimeScope>>().Value),
                new CurrentScopeLifetime(),
                InstanceSharing.Shared,
                InstanceOwnership.ExternallyOwned,
                new Service[] { new TypedService(typeof(ILifetimeScope)), new TypedService(typeof(IComponentContext)) },
                new Dictionary<string, object>()));

            _rootLifetimeScope = new LifetimeScope(_componentRegistry);
            _rootLifetimeScope.Resolve<IndirectReference<ILifetimeScope>>().Value = _rootLifetimeScope;
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return _rootLifetimeScope.BeginLifetimeScope();
        }

        public object Resolve(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return _rootLifetimeScope.Resolve(registration, parameters);
        }

        public IComponentRegistry ComponentRegistry
        {
            get { return _componentRegistry; }
        }

        public IDisposer Disposer
        {
            get { return _rootLifetimeScope.Disposer; }
        }

        public object Tag
        {
            get { return _rootLifetimeScope.Tag; }
            set { _rootLifetimeScope.Tag = value; }
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
