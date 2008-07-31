
namespace Autofac
{
    public class RootScopeLifetime : IComponentLifetime
    {
        public INestedLifetimeScope FindScope(INestedLifetimeScope mostNestedVisibleScope)
        {
            Enforce.ArgumentNotNull(mostNestedVisibleScope, "mostNestedVisibleScope");
            return mostNestedVisibleScope.RootLifetimeScope;
        }
    }
}
