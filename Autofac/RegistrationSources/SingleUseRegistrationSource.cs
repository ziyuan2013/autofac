using System;

namespace Autofac.RegistrationSources
{
    public class SingleUseRegistrationSource : IRegistrationSource
    {
        IConcreteRegistrationData _registrationData;
        bool _used;

        public SingleUseRegistrationSource(IConcreteRegistrationData registrationData)
        {
            _registrationData = Enforce.ArgumentNotNull(registrationData, "registrationData");
        }

        public bool TryGetRegistration(Service service, out IComponentRegistration registration)
        {
            if (_used)
                throw new InvalidOperationException("Reused registration source.");

            if (_registrationData.Services.Contains(service))
            {
                registration = new ComponentRegistration(
                    _registrationData.Id,
                    _registrationData.Activator,
                    _registrationData.Lifetime,
                    _registrationData.Sharing,
                    _registrationData.Ownership,
                    _registrationData.Services,
                    _registrationData.ExtendedProperties);

                _used = true;
                return true;
            }
            else
            {
                registration = null;
                return false;
            }
        }
    }
}
