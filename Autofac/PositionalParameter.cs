using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac
{
    public class PositionalParameter : ConstantParameter
    {
        public int Position { get; private set; }

        public PositionalParameter(int position, object value)
            : base(value)
        {
            if (position < 0) throw new ArgumentOutOfRangeException("position");

            Position = position;
        }

        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            Enforce.ArgumentNotNull(pi, "pi");

            if (pi.Position == Position)
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
