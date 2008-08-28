using System.Collections.Generic;
using Autofac.Registry;
using Autofac.Services;

namespace Autofac
{
    public interface IComponentContext
    {
        IComponentRegistry ComponentRegistry { get; }

        bool TryResolve(Service service, IEnumerable<Parameter> parameters, out object instance);
    }
}
