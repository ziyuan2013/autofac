
namespace Autofac
{
    public class CurrentScopeLifetime : IComponentLifetime
    {
        public INestedLifetimeScope FindScope(INestedLifetimeScope mostNestedVisibleScope)
        {
            Enforce.ArgumentNotNull(mostNestedVisibleScope, "mostNestedVisibleScope");
            return mostNestedVisibleScope;
        }
    }
}
