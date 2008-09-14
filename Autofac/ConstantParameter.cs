using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public abstract class ConstantParameter : Parameter
    {
        public object Value { get; private set; }

        protected ConstantParameter(object value)
        {
            Value = value;
        }
    }
}
