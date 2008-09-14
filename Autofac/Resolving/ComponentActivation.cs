using System;
using System.Collections.Generic;
using Autofac.Registry;
using Autofac.Lifetime;
using Autofac.Services;
using Autofac.Disposal;

namespace Autofac.Resolving
{
    // Is a component context that pins resolution to a point in the context hierarchy
    class ComponentActivation : IComponentContext
    {
        IComponentRegistration _registration;
        IResolveOperation _context;
        ISharingLifetimeScope _activationScope;
        object _newInstance;
        bool _executed;

        public ComponentActivation(
            IComponentRegistration registration,
            IResolveOperation context,
            ISharingLifetimeScope mostNestedVisibleScope)
        {
            _registration = Enforce.ArgumentNotNull(registration, "registration");
            _context = Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(mostNestedVisibleScope, "mostNestedVisibleScope");
            _activationScope = _registration.Lifetime.FindScope(mostNestedVisibleScope);
        }

        public IComponentRegistration Registration { get { return _registration; } }

        public object Execute(IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(parameters, "parameters");
            if (_executed)
                throw new InvalidOperationException("Execute already called.");
            else
                _executed = true;

            object sharedInstance;
            if (IsSharedInstanceAvailable(out sharedInstance))
                return sharedInstance;

            _newInstance = _registration.Activator.ActivateInstance(this, parameters);

            AssignNewInstanceOwnership();

            ShareNewInstance();

            _registration.RaiseActivating(this, _newInstance);

            return _newInstance;
        }

        void ShareNewInstance()
        {
            if (_registration.Sharing == InstanceSharing.Shared)
                _activationScope.AddSharedInstance(_registration.Id, _newInstance);
        }

        void AssignNewInstanceOwnership()
        {
            if (_registration.Ownership == InstanceOwnership.OwnedByLifetimeScope)
            {
                IDisposable instanceAsDisposable = _newInstance as IDisposable;
                if (instanceAsDisposable != null)
                    _activationScope.Disposer.Add(instanceAsDisposable);
            }
        }

        bool IsSharedInstanceAvailable(out object sharedInstance)
        {
            if (_registration.Sharing == InstanceSharing.Shared)
            {
                return _activationScope.TryGetSharedInstance(_registration.Id, out sharedInstance);
            }
            else
            {
                sharedInstance = null;
                return false;
            }
        }

        public void Complete()
        {
            if (_newInstance != null)
            {
                _registration.RaiseActivated(this, _newInstance);
            }
        }

        public IComponentRegistry ComponentRegistry
        {
            get { return _activationScope.ComponentRegistry; }
        }

        public object Resolve(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return _context.Resolve(_activationScope, registration, parameters);
        }
    }
}
