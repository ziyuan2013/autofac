﻿// This software is part of the Autofac IoC container
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
using Autofac.Component.Activation;

namespace Autofac.Registrars.Automatic
{
    /// <summary>
    /// Provides registrations based on a requested type.
    /// </summary>
    public class AutomaticRegistrationHandler : ReflectiveRegistrationSource
    {
        Predicate<Type> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomaticRegistrationHandler"/> class.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="deferredParams">The deferred registration params.</param>
        /// <param name="constructorSelector">The constructor selector.</param>
        /// <param name="arguments">Additional arguments to supply to the component's constructor.</param>
        /// <param name="properties">Additional properties to set on the component.</param>
        public AutomaticRegistrationHandler(
            Predicate<Type> predicate,
            DeferredRegistrationParameters deferredParams,
            IConstructorSelector constructorSelector,
            IEnumerable<Parameter> arguments,
            IEnumerable<NamedPropertyParameter> properties
        )
        : base(deferredParams, constructorSelector, arguments, properties)
        {
            _predicate = Enforce.ArgumentNotNull(predicate, "predicate");
		}

        /// <summary>
        /// Determine if the service represents a type that can be registered, and if so,
        /// retrieve that type as well as the services that the registration should expose.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="implementor">The implementation type.</param>
        /// <param name="services">The services.</param>
        /// <returns>True if a registration can be made.</returns>
        protected override bool TryGetImplementation(Service service, out Type implementor, out IEnumerable<Service> services)
        {
            Enforce.ArgumentNotNull(service, "service");
            implementor = null;
            services = null;

            TypedService typedService = service as TypedService;
            if (typedService == null)
                return false;

            if (typedService.ServiceType.IsAbstract ||
                !_predicate(typedService.ServiceType))
                return false;

            services = new[] { service };
            implementor = typedService.ServiceType;

            return true;
        }
    }
}