using System.Collections.Generic;

namespace Autofac
{
    public interface IComponentContext
    {
        IComponentRegistry ComponentRegistry { get; }

        bool TryResolve(Service service, IEnumerable<Parameter> parameters, out object instance);
    }
}
