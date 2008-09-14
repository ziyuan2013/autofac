using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public static class ParameterExtensions
    {
        public static T Named<T>(this IEnumerable<Parameter> parameters, string name)
        {
            Enforce.ArgumentNotNull(parameters, "parameters");
            Enforce.ArgumentNotNullOrEmpty(name, "name");

            return ConstantValue<NamedParameter, T>(parameters, c => c.Name == name);
        }

        public static T Positional<T>(this IEnumerable<Parameter> parameters, int position)
        {
            Enforce.ArgumentNotNull(parameters, "parameters");
            if (position < 0) throw new ArgumentOutOfRangeException("position");

            return ConstantValue<PositionalParameter, T>(parameters, c => c.Position == position);
        }

        static TValue ConstantValue<TParameter, TValue>(IEnumerable<Parameter> parameters, Func<TParameter, bool> predicate)
            where TParameter : ConstantParameter
        {
            Enforce.ArgumentNotNull(parameters, "parameters");
            Enforce.ArgumentNotNull(predicate, "predicate");

            return parameters
                .OfType<TParameter>()
                .Where(predicate)
                .Select(p => p.Value)
                .Cast<TValue>()
                .First();
        }
    }
}
