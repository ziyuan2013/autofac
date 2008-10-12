using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Injection;

namespace Autofac.Activators
{
    public class MatchingSignatureConstructorSelector : IConstructorSelector
    {
        readonly Type[] _signature;

        public MatchingSignatureConstructorSelector(Type[] signature)
        {
           _signature =  Enforce.ArgumentElementNotNull(signature, "signature");
        }

        public ConstructorParameterBinding SelectConstructorBinding(IEnumerable<ConstructorParameterBinding> constructorBindings)
        {
            Enforce.ArgumentNotNull(constructorBindings, "constructorBindings");
            return constructorBindings
                .Where(b => b.TargetConstructor.GetParameters().Select(p => p.ParameterType).SequenceEqual(_signature))
                .Single();
        }
    }
}
