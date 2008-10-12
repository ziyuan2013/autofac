using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac.Activators
{
    public interface IConstructorFinder
    {
        IEnumerable<ConstructorInfo> FindConstructors(Type targetType);
    }
}
