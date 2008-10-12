using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Injection;

namespace Autofac.Activators
{
    public interface IConstructorSelector
    {
        ConstructorParameterBinding SelectConstructorBinding(IEnumerable<ConstructorParameterBinding> constructorBindings);
    }
}
