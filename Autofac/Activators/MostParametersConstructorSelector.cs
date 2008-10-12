using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Injection;

namespace Autofac.Activators
{
    public class MostParametersConstructorSelector : IConstructorSelector
    {
        public ConstructorParameterBinding SelectConstructorBinding(
            IEnumerable<ConstructorParameterBinding> constructorBindings)
        {
            Enforce.ArgumentNotNull(constructorBindings, "constructorBindings");

            return constructorBindings
                .OrderByDescending(cb => cb.TargetConstructor.GetParameters().Length)
                .First();
        }
    }
}
