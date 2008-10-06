using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.GeneratedFactories;

namespace Autofac.Syntax
{
    public class GeneratedFactoryRegistrar<T> : IGeneratedFactoryRegistrar<T>
    {
        FactoryGenerator<T> _generator;
        IConcreteRegistrar<T> _inner;

        public GeneratedFactoryRegistrar(FactoryGenerator<T> generator, IConcreteRegistrar<T> inner)
        {
            _generator = Enforce.ArgumentNotNull(generator, "generator");
            _inner = Enforce.ArgumentNotNull(inner, "inner");
        }

        public IGeneratedFactoryRegistrar<T> NamedParameterMapping()
        {
            _generator.ParameterMapping = ParameterMapping.ByName;
            return this;
        }

        public IGeneratedFactoryRegistrar<T> PositionalParameterMapping()
        {
            _generator.ParameterMapping = ParameterMapping.ByPosition;
            return this;
        }

        public IGeneratedFactoryRegistrar<T> TypedParameterMapping()
        {
            _generator.ParameterMapping = ParameterMapping.ByType;
            return this;
        }

        public IGeneratedFactoryRegistrar<T> Named(string name)
        {
            _inner.Named(name);
            return this;
        }
    }
}
