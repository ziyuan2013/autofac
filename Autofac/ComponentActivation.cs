using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Internal;

namespace Autofac
{
    public class ComponentActivation
    {
        IComponentRegistration _registration;
        IComponentContext _context;
        INestedLifetimeScope _mostNestedVisibleScope;
        INestedLifetimeScope _activationScope;
        object _newInstance;

        public ComponentActivation(
            IComponentRegistration registration,
            IComponentContext context,
            INestedLifetimeScope mostNestedVisibleScope)
        {
            _registration = Enforce.ArgumentNotNull(registration, "registration");
            _context = Enforce.ArgumentNotNull(context, "context");
            _mostNestedVisibleScope = Enforce.ArgumentNotNull(mostNestedVisibleScope, "mostNestedVisibleScope");
        }

        public object Execute(IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(parameters, "parameters");
            if (_activationScope != null)
                throw new InvalidOperationException("Execute already called.");

            _activationScope = _registration.Lifetime.FindScope(_mostNestedVisibleScope);

            object sharedInstance;
            if (IsSharedInstanceAvailable(out sharedInstance))
                return sharedInstance;

            _newInstance = _registration.Activator.ActivateInstance(_activationScope, parameters);

            AssignNewInstanceOwnership();

            ShareNewInstance();

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
                _registration.Activator.CompleteActivation(_newInstance, _activationScope);
            }
        }

        public INestedLifetimeScope ActivationScope
        {
            get
            {
                if (_activationScope == null)
                    throw new InvalidOperationException("not executed yet");

                return _activationScope;
            }
        }
    }
}
