using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Autofac.Injection;

namespace Autofac.Activators
{
    public class ReflectionActivator : InstanceActivator, IInstanceActivator
    {
        Type _implementationType;

        public ReflectionActivator(Type implementationType)
            : base(Enforce.ArgumentNotNull(implementationType, "implementationType"))
        {
            _implementationType = implementationType;
        }

        public object ActivateInstance(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(parameters, "parameters");

            var constructorBindings = GetConstructorBindings(
                context,
                parameters,
                _implementationType.FindMembers(
                    MemberTypes.Constructor,
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    null).Cast<ConstructorInfo>());

            var selectedBinding = SelectConstructorBinding(constructorBindings);

            return selectedBinding.Instantiate();
        }

        private ConstructorParameterBinding SelectConstructorBinding(
            IEnumerable<ConstructorParameterBinding> constructorBindings)
        {
            Enforce.ArgumentNotNull(constructorBindings, "constructorBindings");
            return constructorBindings.First();
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
