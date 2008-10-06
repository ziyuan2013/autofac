using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Autofac.Services;

namespace Autofac.GeneratedFactories
{
    /// <summary>
    /// Determines how the parameters of the delegate type are passed on
    /// to the generated Resolve() call as Parameter objects.
    /// </summary>
    public enum ParameterMapping { ByName, ByType, ByPosition };

    /// <summary>
    /// Generates context-bound closures that represent factories from
    /// a set of heuristics based on delegate type signatures.
    /// </summary>
    /// <typeparam name="TDelegate">The factory delegate type.</typeparam>
    public class FactoryGenerator<TDelegate>
    {
        IDictionary<ParameterMapping, Func<IComponentContext, IEnumerable<Parameter>, TDelegate>> _generators =
            new Dictionary<ParameterMapping, Func<IComponentContext, IEnumerable<Parameter>, TDelegate>>();

        /// <summary>
        /// Determines how the parameters of the delegate type are passed on
        /// to the generated Resolve() call as Parameter objects.
        /// For Func-based delegates, this defaults to ByType. Otherwise, the
        /// parameters will be mapped by name.
        /// </summary>
        public ParameterMapping ParameterMapping { get; set; }

        /// <summary>
        /// Create a factory generator.
        /// </summary>
        /// <param name="service">The service that will be resolved in
        /// order to create the products of the factory.</param>
        public FactoryGenerator(Service service)
        {
            Enforce.ArgumentNotNull(service, "service");

            var delegateType = typeof(TDelegate);
            Enforce.ArgumentTypeIsFunction(delegateType);

            ParameterMapping = delegateType.Name.StartsWith("Func`") ? ParameterMapping.ByType : ParameterMapping.ByName;

            foreach (var pm in Enum.GetValues(typeof(ParameterMapping)).Cast<ParameterMapping>())
                _generators[pm] = CreateGenerator(service, delegateType, pm);
        }

        Func<IComponentContext, IEnumerable<Parameter>, TDelegate> CreateGenerator(Service service, Type delegateType, ParameterMapping pm)
        {
            // (c, p) => ([dps]*) => (drt)Resolve(c, service, [new NamedParameter(name, (object)dps)]*)

            // (c, p)
            var activatorContextParam = Expression.Parameter(typeof(IComponentContext), "c");
            var activatorParamsParam = Expression.Parameter(typeof(IEnumerable<Parameter>), "p");
            var activatorParams = new[] { activatorContextParam, activatorParamsParam };

            var invoke = delegateType.GetMethod("Invoke");

            // [dps]*
            var creatorParams = invoke
                .GetParameters()
                .Select(pi => Expression.Parameter(pi.ParameterType, pi.Name))
                .ToList();

            Expression[] resolveParameterArray = MapParameters(delegateType, creatorParams, pm);

            // service, [new Parameter(name, (object)dps)]*
            var resolveParams = new Expression[] {
                activatorContextParam,
                Expression.Constant(service),
                Expression.NewArrayInit(typeof(Parameter), resolveParameterArray)
            };

            // c.Resolve(...)
            var resolveCall = Expression.Call(
                typeof(ResolutionExtensions).GetMethod("Resolve", new[] { typeof(IComponentContext), typeof(Service), typeof(Parameter[]) }),
                resolveParams);

            // (drt)
            var resolveCast = Expression.Convert(resolveCall, invoke.ReturnType);

            // ([dps]*) => c.Resolve(service, [new Parameter(name, dps)]*)
            var creator = Expression.Lambda(delegateType, resolveCast, creatorParams);

            // (c, p) => (
            var activator = Expression.Lambda<Func<IComponentContext, IEnumerable<Parameter>, TDelegate>>(creator, activatorParams);

            return activator.Compile();
        }

        Expression[] MapParameters(Type delegateType, List<ParameterExpression> creatorParams, ParameterMapping pm)
        {
            switch(pm)
            {
                case ParameterMapping.ByType:
                    return creatorParams
                            .Select(p => Expression.New(
                                typeof(TypedParameter).GetConstructor(new[] { typeof(Type), typeof(object) }),
                                Expression.Constant(p.Type), Expression.Convert(p, typeof(object))))
                            .OfType<Expression>()
                            .ToArray();

                case ParameterMapping.ByPosition:
                    return creatorParams
                        .Select((p, i) => Expression.New(
                                typeof(PositionalParameter).GetConstructor(new[] { typeof(int), typeof(object) }),
                                Expression.Constant(i), Expression.Convert(p, typeof(object))))
                            .OfType<Expression>()
                            .ToArray();

                case ParameterMapping.ByName:
                default:
                    return creatorParams
                            .Select(p => Expression.New(
                                typeof(NamedParameter).GetConstructor(new[] { typeof(string), typeof(object) }),
                                Expression.Constant(p.Name), Expression.Convert(p, typeof(object))))
                            .OfType<Expression>()
                            .ToArray();
            }
        }

        /// <summary>
        /// Generates a factory delegate that closes over the provided context.
        /// </summary>
        /// <param name="context">The context in which the factory will be used.</param>
        /// <param name="parameters">Parameters provided to the resolve call for the factory itself.</param>
        /// <returns>A factory delegate that will work within the context.</returns>
        public TDelegate GenerateFactory(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(parameters, "parameters");

            return _generators[ParameterMapping](context.Resolve<ILifetimeScope>(), parameters);
        }
    }
}
