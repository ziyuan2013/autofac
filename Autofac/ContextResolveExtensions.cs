using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public static class ContextResolveExtensions
    {
        public static T Resolve<T>(this IComponentContext context)
        {
            return (T)Resolve(context, typeof(T));
        }

        public static object Resolve(this IComponentContext context, Type serviceType)
        {
            return Resolve(context, serviceType, new Parameter[0]);
        }

        public static object Resolve(this IComponentContext context, Type serviceType, IEnumerable<Parameter> parameters)
        {
            return Resolve(context, new TypedService(serviceType), parameters);
        }

        public static object Resolve(this IComponentContext context, Service service)
        {
            return Resolve(context, service, new Parameter[0]);
        }

        public static object Resolve(this IComponentContext context, Service service, IEnumerable<Parameter> parameters)
        {
            object instance;
            var successful = context.TryResolve(service, parameters, out instance);
            if (!successful)
                throw new InvalidOperationException("Not registered..");
            return instance;
        }
    }
}
