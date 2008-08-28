
namespace Autofac.Lifetime
{
    public class RootScopeLifetime : IComponentLifetime
    {
        public ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope)
        {
            Enforce.ArgumentNotNull(mostNestedVisibleScope, "mostNestedVisibleScope");
            return mostNestedVisibleScope.RootLifetimeScope;
        }
    }
}
