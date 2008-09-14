using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Autofac.Registry;
using Autofac.Lifetime;
using Autofac.Services;

namespace Autofac.Resolving
{
    // Is a component context that sequences and monitors the multiple
    // activations that go into producing a single requested object graph
    class ResolveOperation : IComponentContext, IResolveOperation
    {
        Stack<ComponentActivation> _activationStack = new Stack<ComponentActivation>();
        ICollection<ComponentActivation> _successfulActivations;
        ISharingLifetimeScope _mostNestedLifetimeScope;
        CircularDependencyDetector _circularDependencyDetector = new CircularDependencyDetector();
        int _callDepth = 0;

        public ResolveOperation(ISharingLifetimeScope mostNestedLifetimeScope)
        {
            _mostNestedLifetimeScope = Enforce.ArgumentNotNull(mostNestedLifetimeScope, "mostNestedLifetimeScope");
            ResetSuccessfulActivations();
        }

        public object Resolve(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return Resolve(_mostNestedLifetimeScope, registration, parameters);
        }

        public object Resolve(ISharingLifetimeScope activationScope, IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(activationScope, "activationScope");
            Enforce.ArgumentNotNull(registration, "registration");
            Enforce.ArgumentNotNull(parameters, "parameters");

            _circularDependencyDetector.CheckForCircularDependency(registration, _activationStack, ++_callDepth);

            object instance = null;

            var activation = new ComponentActivation(registration, this, activationScope);

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

            if (_activationStack.Count == 0)
                CompleteActivations();

            --_callDepth;

            return instance;
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
            get { return _mostNestedLifetimeScope.ComponentRegistry; }
        }
    }
}
