using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac
{
    public class NamedParameter : ConstantParameter
    {
        public string Name { get; private set; }

        public NamedParameter(string name, object value)
            : base(value, pi => pi.Name == name)
        {
            Name = Enforce.ArgumentNotNullOrEmpty(name, "name");
        }
    }
}
