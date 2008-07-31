using Autofac.Activators;
using Autofac.RegistrationSources;
using Autofac.Syntax;

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

            container.ComponentRegistry.Register(registration, false);
        }

        public static IConcreteRegistrar<T> RegisterInstance<T>(this IContainer container, T instance)
            where T : class
        {
            Enforce.ArgumentNotNull(container, "container");
            Enforce.ArgumentNotNull(instance, "instance");

            var registrar = new ConcreteRegistrar<T>(new ProvidedInstanceActivator(instance));
            container.ComponentRegistry.AddRegistrationSource(new SingleUseRegistrationSource(registrar));
            return registrar;
        }
    }
}
