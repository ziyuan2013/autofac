using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Internal;

namespace Autofac
{
    public static class BuilderExtensions
    {
        public static void RegisterModule(this IContainer container, IModule module)
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(module, "module");

            module.Configure(container);
        }

        public static void RegisterComponent(this IContainer container, IComponentRegistration registration)
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(registration, "registration");

            container.ComponentRegistry.Register(registration, false);
        }
    }
}
