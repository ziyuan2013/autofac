using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac
{
    public class PositionalParameter : ConstantParameter
    {
        public int Position { get; private set; }

        public PositionalParameter(int position, object value)
            : base(value, pi => pi.Position == position)
        {
            if (position < 0) throw new ArgumentOutOfRangeException("position");

            Position = position;
        }
    }
}
