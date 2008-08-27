
namespace Autofac
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
