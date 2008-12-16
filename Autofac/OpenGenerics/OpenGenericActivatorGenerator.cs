using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.RegistrationSources;
using Autofac.Syntax;
using Autofac.Activators;
using Autofac.Services;
using System.Reflection;

namespace Autofac.OpenGenerics
{
    class OpenGenericActivatorGenerator : IReflectiveActivatorData, IDynamicActivatorGenerator
    {
        Type _implementationType;
        IConstructorFinder _constructorFinder;
        IConstructorSelector _constructorSelector;

        public OpenGenericActivatorGenerator(Type implementationType)
        {
            _implementationType = implementationType;
            _constructorFinder = new BindingFlagsConstructorFinder(BindingFlags.Public);
            _constructorSelector = new MostParametersConstructorSelector();
        }

        public void SetConstructorFinder(IConstructorFinder constructorFinder)
        {
            _constructorFinder = Enforce.ArgumentNotNull(constructorFinder, "constructorFinder");
        }

        public void SetConstructorSelector(IConstructorSelector constructorSelector)
        {
            _constructorSelector = Enforce.ArgumentNotNull(constructorSelector, "constructorSelector");
        }

        public bool TryGenerateActivator(Service service, IEnumerable<Service> configuredServices, out IInstanceActivator activator, out IEnumerable<Service> services)
        {
            throw new NotImplementedException();
        }
    }
}
