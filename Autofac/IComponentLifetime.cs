
namespace Autofac
{
    public interface IComponentLifetime
    {
        ISharingLifetimeScope FindScope(ISharingLifetimeScope mostNestedVisibleScope);
    }
}
