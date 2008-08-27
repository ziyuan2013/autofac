using System;

namespace Autofac.RegistrationSources
{
    public class DeferredRegistrationSource : IDeferredRegistrationSource
    {
        IConcreteRegistrationData _registrationData;
        bool _used;

        public DeferredRegistrationSource(IConcreteRegistrationData registrationData)
        {
            _registrationData = Enforce.ArgumentNotNull(registrationData, "registrationData");
        }

        public IComponentRegistration GetRegistration()
        {
            if (_used)
                throw new InvalidOperationException("Reused registration source.");

            var registration = new ComponentRegistration(
                _registrationData.Id,
                _registrationData.Activator,
                _registrationData.Lifetime,
                _registrationData.Sharing,
                _registrationData.Ownership,
                _registrationData.Services,
                _registrationData.ExtendedProperties);

            _used = true;
            return registration;
        }
    }
}
