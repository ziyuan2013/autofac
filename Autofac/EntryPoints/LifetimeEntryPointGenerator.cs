using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Autofac.EntryPoints
{
    public class LifetimeEntryPointGenerator<TDelegate>
    {
        Func<ILifetimeScope, TDelegate> _entryPointGenerator;

        public LifetimeEntryPointGenerator(Func<IComponentContext, IEnumerable<Parameter>, object> entryPointBinding, object scopeTag)
        {
            Enforce.ArgumentNotNull(entryPointBinding, "entryPointBinding");

            Func<ILifetimeScope, IEnumerable<Parameter>, object> entryPoint = (ls, p) =>
            {
                using (var inner = ls.BeginLifetimeScope())
                {
                    if (scopeTag != null)
                        inner.Tag = scopeTag;

                    return entryPointBinding(inner, p);
                }
            };

            // s => a, b, c => (drt) entryPoint(s, new PositionalParameter(0, a), ...)

            var outerScopeParam = Expression.Parameter(typeof(ILifetimeScope), "s");
            var generatorParams = new[] { outerScopeParam };

            var invoke = typeof(TDelegate).GetMethod("Invoke");

            // a, b, c
            var entryPointParams = invoke
                .GetParameters()
                .Select(pi => Expression.Parameter(pi.ParameterType, pi.Name))
                .ToList();

            var mappedEntryPointParams = entryPointParams
                .Select((p, i) => Expression.New(
                        typeof(PositionalParameter).GetConstructor(new[] { typeof(int), typeof(object) }),
                        Expression.Constant(i), Expression.Convert(p, typeof(object))))
                .OfType<Expression>()
                .ToArray();

            var entryPointBindingParams = new Expression[] { 
                outerScopeParam,
                Expression.NewArrayInit(typeof(Parameter), mappedEntryPointParams)
            };

            var entryPointInvoke = entryPoint.GetType().GetMethod("Invoke");

            // entryPoint(...)
            var entryPointCall = Expression.Call(
                Expression.Constant(entryPoint),
                entryPointInvoke,
                entryPointBindingParams);

            // (drt)
            var resultCast = Expression.Convert(entryPointCall, invoke.ReturnType);

            // ([dps]*) => entryPoint(s, [new Parameter(name, dps)]*)
            var entryPointExpr = Expression.Lambda(typeof(TDelegate), resultCast, entryPointParams);

            // (c, p) => (
            var generator = Expression.Lambda<Func<ILifetimeScope, TDelegate>>(entryPointExpr, generatorParams);

            _entryPointGenerator = generator.Compile();
        }

        public TDelegate GenerateEntryPoint(IComponentContext context, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(parameters, "parameters");

            var currentScope = context.Resolve<ILifetimeScope>();

            return _entryPointGenerator(currentScope);
        }
    }
}
