﻿// This software is part of the Autofac IoC container
// Copyright (c) 2010 Autofac Contributors
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

using System.Collections.Generic;
using Autofac.Core;

namespace Autofac.Builder
{
    /// <summary>
    /// Generates activators in an IRegistrationSource.
    /// </summary>
    /// <typeparam name="TActivatorData">Data associated with the specific kind of activator.</typeparam>
#if !(SL2 || SL3 || NET35) 
    public interface IActivatorGenerator<in TActivatorData>
#else
    public interface IActivatorGenerator<TActivatorData>
#endif
    {
        /// <summary>
        /// Given a requested service and registration data, attempt to generate an
        /// activator for the service.
        /// </summary>
        /// <param name="service">Service that was requested.</param>
        /// <param name="configuredServices">Services associated with the activator generator.</param>
        /// <param name="reflectionActivatorData">Data specific to this kind of activator.</param>
        /// <param name="activator">Resulting activator.</param>
        /// <param name="services">Services provided by the activator.</param>
        /// <returns>True if an activator could be generated.</returns>
        bool TryGenerateActivator(
            Service service,
            IEnumerable<Service> configuredServices,
            TActivatorData reflectionActivatorData,
            out IInstanceActivator activator,
            out IEnumerable<Service> services);
    }
}
