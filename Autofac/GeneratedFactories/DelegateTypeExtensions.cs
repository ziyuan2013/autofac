using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.GeneratedFactories
{
    static class DelegateTypeExtensions
    {
        public static bool IsFunction(this Type type)
        {
            Enforce.ArgumentNotNull(type, "type");
            return type.GetMethod("Invoke") != null;
        }

        public static Type FunctionReturnType(this Type type)
        {
            Enforce.ArgumentNotNull(type, "type");
            var invoke = type.GetMethod("Invoke");
            Enforce.NotNull(invoke);
            return invoke.ReturnType;
        }
    }
}
