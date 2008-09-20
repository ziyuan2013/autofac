using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac
{
    // Exact type matching. May provide polymorphic type matching in separate param type
    public class TypedParameter : ConstantParameter
    {
        public Type Type { get; private set; }

        public TypedParameter(Type type, object value)
            : base(value, pi => pi.ParameterType == type)
        {
            Type = Enforce.ArgumentNotNull(type, "type");
        }
    }
}
