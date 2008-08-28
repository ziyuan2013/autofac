using System;
using Autofac.Activators;

namespace Autofac.RegistrationSources
{
    public interface IConcreteRegistrationData : IRegistrationData
    {
        Guid Id { get; }
        IInstanceActivator Activator { get; }
    }
}
