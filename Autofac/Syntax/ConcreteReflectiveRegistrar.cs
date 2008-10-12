using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Activators;
using Autofac.Services;
using Autofac.Events;
using System.Reflection;

namespace Autofac.Syntax
{
    public class ConcreteReflectiveRegistrar<T> : IConcreteReflectiveRegistrar<T>
    {
        ConcreteRegistrar<T> _inner;
        ReflectionActivator _activator;
        
        public ConcreteReflectiveRegistrar(ConcreteRegistrar<T> inner, ReflectionActivator activator)
        {
            _inner = Enforce.ArgumentNotNull(inner, "inner");
            _activator = Enforce.ArgumentNotNull(activator, "activator");
        }

        public IConcreteReflectiveRegistrar<T> ExternallyOwned()
        {
            _inner.ExternallyOwned();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> OwnedByLifetimeScope()
        {
            _inner.OwnedByLifetimeScope();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> UnsharedInstances()
        {
            _inner.UnsharedInstances();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> SingleInstance()
        {
            _inner.SingleInstance();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> InstancePerLifetimeScope()
        {
            _inner.InstancePerLifetimeScope();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> InstancePer(object lifetimeScopeTag)
        {
            _inner.InstancePer(lifetimeScopeTag);
            return this;
        }

        public IConcreteReflectiveRegistrar<T> As<TService>()
        {
            _inner.As<TService>();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> As<TService1, TService2>()
        {
            _inner.As<TService1, TService2>();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> As<TService1, TService2, TService3>()
        {
            _inner.As<TService1, TService2, TService3>();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> As(params Service[] services)
        {
            _inner.As(services);
            return this;
        }

        public IConcreteReflectiveRegistrar<T> Named(string name)
        {
            _inner.Named(name);
            return this;
        }

        public IConcreteReflectiveRegistrar<T> PropertiesAutowired()
        {
            _inner.PropertiesAutowired();
            return this;
        }

        public IConcreteReflectiveRegistrar<T> PropertiesAutowired(bool allowCircularDependencies)
        {
            _inner.PropertiesAutowired(allowCircularDependencies);
            return this;
        }

        public IConcreteReflectiveRegistrar<T> OnActivating(Action<ActivatingEventArgs<T>> e)
        {
            _inner.OnActivating(e);
            return this;
        }

        public IConcreteReflectiveRegistrar<T> OnActivated(Action<ActivatedEventArgs<T>> e)
        {
            _inner.OnActivated(e);
            return this;
        }

        public IConcreteReflectiveRegistrar<T> FindConstructorsWith(BindingFlags bindingFlags)
        {
            ResetActivator(new ReflectionActivator(
                _activator.BestGuessImplementationType,
                new BindingFlagsConstructorFinder(bindingFlags),
                _activator.ConstructorSelector));
            
            return this;
        }

        private void ResetActivator(ReflectionActivator reflectionActivator)
        {
            _inner.Activator = _activator = reflectionActivator;
        }

        public IConcreteReflectiveRegistrar<T> FindConstructorsWith(IConstructorFinder constructorFinder)
        {
            ResetActivator(new ReflectionActivator(
                _activator.BestGuessImplementationType,
                constructorFinder,
                _activator.ConstructorSelector));

            return this;
        }

        public IConcreteReflectiveRegistrar<T> UsingConstructor(params Type[] signature)
        {
            ResetActivator(new ReflectionActivator(
                _activator.BestGuessImplementationType,
                _activator.ConstructorFinder,
                new MatchingSignatureConstructorSelector(signature)));

            return this;
        }

        public IConcreteReflectiveRegistrar<T> UsingConstructorSelector(IConstructorSelector constructorSelector)
        {
            ResetActivator(new ReflectionActivator(
                _activator.BestGuessImplementationType,
                _activator.ConstructorFinder,
                constructorSelector));

            return this;
        }
    }
}
