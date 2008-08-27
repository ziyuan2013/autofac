using System.Collections.Generic;
using System.Linq;

namespace Autofac
{
    class ResolveOperation : IComponentContext
    {
        IComponentRegistry _componentRegistry;
        Stack<ComponentActivation> _activationStack = new Stack<ComponentActivation>();
        ICollection<ComponentActivation> _successfulActivations;
        ISharingLifetimeScope _mostNestedLifetimeScope;
        
        public ResolveOperation(ISharingLifetimeScope mostNestedLifetimeScope, IComponentRegistry componentRegistry)
        {
            _mostNestedLifetimeScope = Enforce.ArgumentNotNull(mostNestedLifetimeScope, "mostNestedLifetimeScope");
            _componentRegistry = Enforce.ArgumentNotNull(componentRegistry, "componentRegistry");
            ResetSuccessfulActivations();
        }

        ISharingLifetimeScope CurrentActivationScope
        {
            get
            {
                if (_activationStack.Any())
                    return _activationStack.Peek().ActivationScope;
                else
                    return _mostNestedLifetimeScope;
            }
        }

        public bool TryResolve(Service service, IEnumerable<Parameter> parameters, out object instance)
        {
            Enforce.ArgumentNotNull(service, "service");
            Enforce.ArgumentNotNull(parameters, "parameters");

            IComponentRegistration registration;
            if (!_componentRegistry.TryGetRegistration(service, out registration))
            {
                instance = null;
                return false;
            }

            var activation = new ComponentActivation(registration, this, CurrentActivationScope);

            _activationStack.Push(activation);
            try
            {
                instance = activation.Execute(parameters);
                _successfulActivations.Add(activation);
            }
            finally
            {
                _activationStack.Pop();
            }

            CompleteActivations();

            return true;
        }

        void CompleteActivations()
        {
            var completed = _successfulActivations;
            ResetSuccessfulActivations();

            foreach (var activation in completed)
                activation.Complete();
        }

        void ResetSuccessfulActivations()
        {
            _successfulActivations = new List<ComponentActivation>();
        }

        public IComponentRegistry ComponentRegistry
        {
            get { return _componentRegistry; }
        }
    }
}
