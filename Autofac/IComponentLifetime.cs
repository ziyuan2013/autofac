
namespace Autofac
{
    public interface IComponentLifetime
    {
        INestedLifetimeScope FindScope(INestedLifetimeScope mostNestedVisibleScope);
    }
}
