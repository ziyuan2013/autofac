
namespace Autofac.Lifetime
{
    public interface IComponentLifetime
    {
        ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope);
    }
}
