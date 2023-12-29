using console_explorer.Services;

namespace console_explorer.Factories
{
    public interface IItemsRendererFactory
    {
        IItemsRenderer CreateGridRenderer(int maxItems);
    }
}