using Spectre.Console;

namespace console_explorer.Services
{
    public class TreeRenderer : ItemsRenderer
    {
        private readonly IExplorerUi explorerUi;
        private readonly int maxItems;

        public TreeRenderer(IExplorerUi explorerUi, int maxItems)
        {
            this.explorerUi = explorerUi;
            this.maxItems = maxItems;
        }

        public override void RenderItems(IEnumerable<FileSystemInfo> items, int selectedIndex)
        {
            var i = 0;

            var itemList = items.ToList();
            var first = itemList.First();

            var root = GetMarkup(first.FullName, i, selectedIndex);
            var tree = new Tree(root);

            foreach (var item in items.Skip(1))
            {
                ++i;

                if (i - selectedIndex < -maxItems)
                {
                    continue;
                }

                var postfix = item is DirectoryInfo ? Path.DirectorySeparatorChar.ToString() : string.Empty;
                var name = new Markup($"[white{BgColor(i, selectedIndex)}]{item.Name}{postfix}[/]");
                tree.AddNode(name);
            }

            explorerUi.UpdateLeft(tree);
        }
    }
}
