using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Autofac
{
    public interface IComponentContext
    {
        IComponentRegistry ComponentRegistry { get; }

        bool TryResolve(Service service, IEnumerable<Parameter> parameters, out object instance);
    }
}
