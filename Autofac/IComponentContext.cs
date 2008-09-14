using System.Collections.Generic;
using Autofac.Registry;
using Autofac.Services;

namespace Autofac
{
    public interface IComponentContext
    {
        IComponentRegistry ComponentRegistry { get; }

        object Resolve(IComponentRegistration registration, IEnumerable<Parameter> parameters);
    }
}
