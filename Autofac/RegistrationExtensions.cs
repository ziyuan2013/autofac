using Autofac.Activators;
using Autofac.RegistrationSources;
using Autofac.Syntax;
using System;
using System.Collections.Generic;

namespace Autofac
{
    public static class RegistrationExtensions
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

            container.ComponentRegistry.Register(registration);
        }

        public static IConcreteRegistrar<T> RegisterInstance<T>(this IContainer container, T instance)
            where T : class
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(instance, "instance");

            return CreateConcreteRegistration<T>(container, new ProvidedInstanceActivator(instance));
        }

        static IConcreteRegistrar<T> CreateConcreteRegistration<T>(IContainer container, IInstanceActivator activator)
            where T : class
        {
            var registrar = new ConcreteRegistrar<T>(activator);
            container.ComponentRegistry.AddDeferredRegistrationSource(new DeferredRegistrationSource(registrar));
            return registrar;
        }

        public static IConcreteRegistrar<T> RegisterType<T>(this IContainer container)
            where T : class
        {
            Enforce.ArgumentNotNull(container, "container");

            return CreateConcreteRegistration<T>(container, new ReflectionActivator(typeof(T)));
        }

        public static IConcreteRegistrar<object> RegisterType(this IContainer container, Type implementationType)
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(implementationType, "implementationType");

            return CreateConcreteRegistration<object>(container, new ReflectionActivator(implementationType));
        }

        public static IConcreteRegistrar<T> RegisterDelegate<T>(this IContainer container, Func<IComponentContext, T> creationDelegate)
            where T : class
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(creationDelegate, "creationDelegate");

            return RegisterDelegate<T>(container, (c, p) => creationDelegate(c));
        }

        public static IConcreteRegistrar<T> RegisterDelegate<T>(this IContainer container, Func<IComponentContext, IEnumerable<Parameter>, T> creationDelegate)
            where T : class
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(creationDelegate, "creationDelegate");

            return CreateConcreteRegistration<T>(container, new DelegateActivator(typeof(T), (c, p) => creationDelegate(c, p)));
        }
    }
}
