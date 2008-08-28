using System;
using System.Linq.Expressions;
using System.Globalization;

namespace Autofac
{
    public class MatchingScopeLifetime : IComponentLifetime
    {
        Func<ILifetimeScope, bool> _matcher;
        string _matchExpressionCode;

        public MatchingScopeLifetime(Expression<Func<ILifetimeScope, bool>> matchExpression)
        {
            Enforce.ArgumentNotNull(matchExpression, "matchExpression");
            _matcher = matchExpression.Compile();
            _matchExpressionCode = matchExpression.Body.ToString();
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

            throw new DependencyResolutionException(string.Format(
                CultureInfo.CurrentCulture, MatchingScopeLifetimeResources.MatchingScopeNotFound, _matchExpressionCode));
        }
    }
}
