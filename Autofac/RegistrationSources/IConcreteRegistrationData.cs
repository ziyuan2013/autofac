using System;

namespace Autofac.RegistrationSources
{
    public interface IConcreteRegistrationData : IRegistrationData
    {
        Guid Id { get; }
        IInstanceActivator Activator { get; }
    }
}
