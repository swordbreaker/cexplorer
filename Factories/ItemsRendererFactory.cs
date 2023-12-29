using console_explorer.Services;

namespace console_explorer.Factories
{
    public class ItemsRendererFactory : IItemsRendererFactory
    {
        private readonly IExplorerUi explorerUI;

        public ItemsRendererFactory(IExplorerUi explorerUI)
        {
            this.explorerUI = explorerUI;
        }

        public IItemsRenderer CreateGridRenderer(int maxItems) =>
            new GridRenderer(explorerUI, maxItems);

        public ItemsRenderer CreateTreeRenderer(int maxItems) =>
            new TreeRenderer(explorerUI, maxItems);
    }
}