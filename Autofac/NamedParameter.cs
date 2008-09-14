using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac
{
    public class NamedParameter : ConstantParameter
    {
        public string Name { get; private set; }

        public NamedParameter(string name, object value)
            : base(value)
        {
            Name = Enforce.ArgumentNotNullOrEmpty(name, "name");
        }

        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            Enforce.ArgumentNotNull(pi, "pi");

            if (pi.Name == Name)
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
