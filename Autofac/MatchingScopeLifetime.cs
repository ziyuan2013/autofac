using System;

namespace Autofac
{
    public class MatchingScopeLifetime : IComponentLifetime
    {
        Predicate<ILifetimeScope> _matcher;

        public MatchingScopeLifetime(Predicate<ILifetimeScope> matcher)
        {
            _matcher = Enforce.ArgumentNotNull(matcher, "matcher");
        }

        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
            Enforce.ArgumentNotNull(mostNestedVisibleScope, "mostNestedVisibleScope");

            var next = mostNestedVisibleScope;
            while (next != null)
            {
                if (_matcher(next))
                    return next;

                next = next.ParentLifetimeScope;
            }

            throw new InvalidOperationException("no match");
        }
    }
}
