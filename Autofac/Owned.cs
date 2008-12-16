using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Disposal;

namespace Autofac
{
    public class Owned<T> : Disposable
    {
        readonly T _value;
        readonly IDisposable _lifetime;

        public Owned(T value, IDisposable lifetime)
        {
            _value = value;
            _lifetime = lifetime;
        }

        public T Value
        {
            get
            {
                CheckNotDisposed();
                return _value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _lifetime.Dispose();

            base.Dispose(disposing);
        }
    }
}
