using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Autofac.Services;

namespace Autofac.GeneratedFactories
{
    public class FactoryGenerator<TDelegate>
    {
        Func<IComponentContext, IEnumerable<Parameter>, TDelegate> _generator;

        public FactoryGenerator(Service service)
        {
            Enforce.ArgumentNotNull(service, "service");

            var delegateType = typeof(TDelegate);
            Enforce.ArgumentTypeIsDelegate(delegateType);

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

            Expression[] resolveParameterArray = MapParametersForDelegateType(delegateType, creatorParams);

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

            _generator = activator.Compile();
        }

        Expression[] MapParametersForDelegateType(Type delegateType, List<ParameterExpression> creatorParams)
        {
            if (delegateType.Name.StartsWith("Func`"))
            {
                return creatorParams
                        .Select(p => Expression.New(
                            typeof(TypedParameter).GetConstructor(new[] { typeof(Type), typeof(object) }),
                            Expression.Constant(p.Type), Expression.Convert(p, typeof(object))))
                        .OfType<Expression>()
                        .ToArray();
            }
            else
            {
                return creatorParams
                        .Select(p => Expression.New(
                            typeof(NamedParameter).GetConstructor(new[] { typeof(string), typeof(object) }),
                            Expression.Constant(p.Name), Expression.Convert(p, typeof(object))))
                        .OfType<Expression>()
                        .ToArray();
            }
        }

        public TDelegate GenerateFactory(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(parameters, "parameters");

            return _generator(context.Resolve<ILifetimeScope>(), parameters);
        }
    }
}
