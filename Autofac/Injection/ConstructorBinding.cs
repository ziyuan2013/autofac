using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac.Injection
{
    public class ConstructorBinding
    {
        Func<object>[] _valueRetrievers;

        /// <summary>
        /// The constructor on the target type. The actual constructor used
        /// might differ, e.g. if using a dynamic proxy.
        /// </summary>
        public ConstructorInfo TargetConstructor { get; private set; }

        public bool CanInstantiate { get; private set; }

        // Ugly, too much work in ctor
        public ConstructorBinding(
            ConstructorInfo ci,
            IEnumerable<Parameter> availableParameters,
            IComponentContext context)
        {
            CanInstantiate = true;
            TargetConstructor = ci;

            var parameters = ci.GetParameters();
            _valueRetrievers = new Func<object>[parameters.Length];

            for (int i = 0; i < parameters.Length; ++i)
            {
                var pi = parameters[i];
                bool foundValue = false;
                foreach (var param in availableParameters)
                {
                    Func<object> valueRetriever = null;
                    if (param.CanSupplyValue(pi, context, out valueRetriever))
                    {
                        _valueRetrievers[i] = valueRetriever;
                        foundValue = true;
                        break;
                    }
                }
                if (!foundValue)
                {
                    CanInstantiate = false;
                    break;
                }
            }
        }

        public object Instantiate()
        {
            if (!CanInstantiate)
                throw new InvalidOperationException();

            var values = new object[_valueRetrievers.Length];
            for (int i = 0; i < _valueRetrievers.Length; ++i)
                values[i] = _valueRetrievers[i].Invoke();

            return TargetConstructor.Invoke(values);
        }
    }
}
