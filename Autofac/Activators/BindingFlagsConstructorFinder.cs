using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Autofac.Activators
{
    public class BindingFlagsConstructorFinder : IConstructorFinder
    {
        readonly BindingFlags _bindingFlags;

        public BindingFlagsConstructorFinder(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public IEnumerable<ConstructorInfo> FindConstructors(Type targetType)
        {
            return targetType.FindMembers(
                                MemberTypes.Constructor,
                                BindingFlags.Instance | _bindingFlags,
                                null,
                                null).Cast<ConstructorInfo>();
        }
    }
}
