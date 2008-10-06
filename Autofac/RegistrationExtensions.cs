using Autofac.Activators;
using Autofac.RegistrationSources;
using Autofac.Syntax;
using System;
using System.Collections.Generic;
using Autofac.Registry;
using Autofac.Services;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Globalization;
using Autofac.GeneratedFactories;

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

        /// <summary>
        /// Registers the factory delegate.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <param name="service">The service that the delegate will return instances of.</param>
        /// <returns>A registrar allowing configuration to continue.</returns>
        public static IGeneratedFactoryRegistrar<TDelegate> RegisterGeneratedFactory<TDelegate>(this IContainer container, Service service)
            where TDelegate : class
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(service, "service");

            var factory = new FactoryGenerator<TDelegate>(service);

            var inner = container.RegisterDelegate<TDelegate>((c, p) => factory.GenerateFactory(c, p))
                .InstancePerLifetimeScope();

            return new GeneratedFactoryRegistrar<TDelegate>(factory, inner);
        }
        /// <summary>
        /// Registers the factory delegate.
        /// </summary>
        /// <typeparam name="TDelegate">The type of the delegate.</typeparam>
        /// <returns>A registrar allowing configuration to continue.</returns>
        public static IGeneratedFactoryRegistrar<TDelegate> RegisterGeneratedFactory<TDelegate>(this IContainer container)
            where TDelegate : class
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentTypeIsFunction(typeof(TDelegate));

            var returnType = typeof(TDelegate).GetMethod("Invoke").ReturnType;
            return container.RegisterGeneratedFactory<TDelegate>(new TypedService(returnType));
        }

        //public static IReflectiveRegistrar RegisterGenericType(
    }
}
