﻿// This software is part of the Autofac IoC container
// Copyright © 2011 Autofac Contributors
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
using Autofac.Util;

namespace Autofac.Core
{
    /// <summary>
    /// Identifies a service using a key in addition to its type.
    /// </summary>
    public sealed class KeyedService : Service, IServiceWithType
    {
        readonly object _serviceKey;
        readonly Type _serviceType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Autofac.Core.KeyedService"/> class.
        /// </summary>
        /// <param name="serviceKey">Key of the service.</param>
        /// <param name="serviceType">Type of the service.</param>
        public KeyedService(object serviceKey, Type serviceType)
        {
            _serviceKey = Enforce.ArgumentNotNull(serviceKey, "serviceKey");
            _serviceType = Enforce.ArgumentNotNull(serviceType, "serviceType");
        }

        /// <summary>
        /// Gets or sets the key of the service.
        /// </summary>
        /// <value>The key of the service.</value>
        public object ServiceKey
        {
            get { return _serviceKey; }
        }

        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>The type of the service.</value>
        public Type ServiceType
        {
            get { return _serviceType; }
        }

        /// <summary>
        /// Gets a human-readable description of the service.
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return ServiceKey + " (" + ServiceType.FullName + ")";
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            var that = obj as KeyedService;

            if (that == null)
                return false;

            return ServiceKey.Equals(that.ServiceKey) && ServiceType.IsCompatibleWith(that.ServiceType);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return ServiceKey.GetHashCode() ^ ServiceType.GetCompatibleHashCode();
        }

        /// <summary>
        /// Return a new service of the same kind, but carrying
        /// <paramref name="newType"/> as the <see cref="ServiceType"/>.
        /// </summary>
        /// <param name="newType">The new service type.</param>
        /// <returns>A new service with the service type.</returns>
        public Service ChangeType(Type newType)
        {
            if (newType == null) throw new ArgumentNullException("newType");
            return new KeyedService(ServiceKey, newType);
        }
    }
}
