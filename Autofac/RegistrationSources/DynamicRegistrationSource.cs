// This software is part of the Autofac IoC container
// Copyright (c) 2007 - 2008 Autofac Contributors
// http://autofac.org
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using Autofac.Activators;
using Autofac.Registry;
using Autofac.Services;

namespace Autofac.RegistrationSources
{
    /// <summary>
    /// This class provides a common base for registration handlers that provide
    /// reflection-based registrations.
    /// </summary>
    public class DynamicRegistrationSource : IDynamicRegistrationSource
    {
        IRegistrationData _registrationData;
        IDynamicActivatorGenerator _activatorGenerator;

        public DynamicRegistrationSource(IRegistrationData registrationData, IDynamicActivatorGenerator activatorGenerator)
        {
            _registrationData = Enforce.ArgumentNotNull(registrationData, "registrationData");
            _activatorGenerator = Enforce.ArgumentNotNull(activatorGenerator, "activatorGenerator");
        }
        
        /// <summary>
        /// Retrieve a registration for an unregistered service, to be used
        /// by the container.
        /// </summary>
        /// <param name="service">The service that was requested.</param>
        /// <param name="registration">A registration providing the service.</param>
        /// <returns>True if the registration could be created.</returns>
        public virtual bool TryGetRegistration(Service service, Func<Service, bool> registeredServicesTest, out IComponentRegistration registration)
        {
            Enforce.ArgumentNotNull(service, "service");
            registration = null;

            IInstanceActivator activator;
            IEnumerable<Service> services;
            if (!_activatorGenerator.TryGenerateActivator(service, _registrationData.Services, out activator, out services))
                return false;

            registration = new ComponentRegistration(
                Guid.NewGuid(),
                activator,
                _registrationData.Lifetime,
                _registrationData.Sharing,
                _registrationData.Ownership,
                services,
                _registrationData.ExtendedProperties);

            foreach (var activatingHandler in _registrationData.ActivatingHandlers)
                registration.Activating += activatingHandler;

            foreach (var activatedHandler in _registrationData.ActivatedHandlers)
                registration.Activated += activatedHandler;

            return true;
        }
    }
}
