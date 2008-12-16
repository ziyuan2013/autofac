using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Activators;

namespace Autofac.Syntax
{
    public interface IReflectiveActivatorData
    {
        void SetConstructorFinder(IConstructorFinder constructorFinder);
        void SetConstructorSelector(IConstructorSelector constructorSelector);
    }
}
