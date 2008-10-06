using System.Reflection;
using System;

namespace Autofac
{
    public abstract class Parameter
    {
        public abstract bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider);
    }
}
