using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Autofac.Injection;

namespace Autofac.Activators
{
    public class ReflectionActivator : InstanceActivator, IInstanceActivator
    {
        readonly Type _implementationType;
        readonly IConstructorSelector _constructorSelector;
        readonly IConstructorFinder _constructorFinder;

        public ReflectionActivator(Type implementationType)
            : this(
                implementationType,
                new BindingFlagsConstructorFinder(BindingFlags.Public),
                new MostParametersConstructorSelector())
        {
        }

        public ReflectionActivator(
            Type implementationType,
            IConstructorFinder constructorFinder,
            IConstructorSelector constructorSelector)
            : base(Enforce.ArgumentNotNull(implementationType, "implementationType"))
        {
            _implementationType = implementationType;
            _constructorFinder = Enforce.ArgumentNotNull(constructorFinder, "constructorFinder");
            _constructorSelector = Enforce.ArgumentNotNull(constructorSelector, "constructorSelector");
        }

        public IConstructorFinder ConstructorFinder
        {
            get { return _constructorFinder; }
        }

        public IConstructorSelector ConstructorSelector
        {
            get { return _constructorSelector; }
        }

        public object ActivateInstance(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(parameters, "parameters");

            var availableConstructors = _constructorFinder.FindConstructors(_implementationType);

            if (!availableConstructors.Any())
                throw new DependencyResolutionException("TODO- No constructors are available.");

            var constructorBindings = GetConstructorBindings(
                context,
                parameters,
                availableConstructors);

            var validBindings = constructorBindings.Where(cb => cb.CanInstantiate);

            if (!validBindings.Any())
                throw new DependencyResolutionException("TODO- get reasons from bindings.");

            var selectedBinding = _constructorSelector.SelectConstructorBinding(validBindings);

            return selectedBinding.Instantiate();
        }

        private IEnumerable<ConstructorParameterBinding> GetConstructorBindings(
            IComponentContext context,
            IEnumerable<Parameter> parameters,
            IEnumerable<ConstructorInfo> constructorInfo)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(parameters, "parameters");
            Enforce.ArgumentNotNull(constructorInfo, "constructorInfo");

            // TODO: also concat with configured parameters...
            var prioritisedParameters =
                parameters.Concat(new Parameter[] { new AutowiringParameter() });

            return constructorInfo.Select(
                ci => new ConstructorParameterBinding(ci, prioritisedParameters, context));
        }
    }
}
