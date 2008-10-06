using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac.Syntax
{
    public interface IGeneratedFactoryRegistrar<T>
    {
        IGeneratedFactoryRegistrar<T> NamedParameterMapping();
        IGeneratedFactoryRegistrar<T> PositionalParameterMapping();
        IGeneratedFactoryRegistrar<T> TypedParameterMapping();
        IGeneratedFactoryRegistrar<T> Named(string name);
    }
}
