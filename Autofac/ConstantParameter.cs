using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac
{
    public abstract class ConstantParameter : Parameter
    {
        Predicate<ParameterInfo> _predicate;

        public object Value { get; private set; }

        protected ConstantParameter(object value, Predicate<ParameterInfo> predicate)
        {
            Value = value;
            _predicate = Enforce.ArgumentNotNull(predicate, "predicate");
        }

        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            Enforce.ArgumentNotNull(pi, "pi");

            if (_predicate(pi))
            {
                valueProvider = () => Value;
                return true;
            }
            else
            {
                valueProvider = null;
                return false;
            }
        }
    }
}
