using Spectre.Console.Rendering;
using Spectre.Console;

namespace console_explorer.Services
{
    public class GridRenderer : ItemsRenderer
    {
        private readonly IExplorerUi explorerUi;
        private readonly int maxItems;

        public GridRenderer(IExplorerUi explorerUi, int maxItems)
        {
            this.explorerUi = explorerUi;
            this.maxItems = maxItems;
        }

        public override void RenderItems(IEnumerable<FileSystemInfo> items, int selectedIndex, string title)
        {
            var grid = new Grid();
            grid.AddColumn();
            grid.AddColumn();

            //grid.AddRow(
            //    "[bold]Icon[/]",
            //    "[bold]Name[/],",
            //    "[bold]Size[/]");

            var i = 0;
            foreach (var item in items)
            {
                if (i - selectedIndex < -maxItems)
                {
                    ++i;
                    continue;
                }


                if (i == 0)
                {
                    var name = new Markup($"[white{BgColor(i, selectedIndex)}]{item.FullName}[/]");
                    grid.AddRow(new Markup(Emoji.Known.OpenFileFolder), name);
                    
                }
                else if(item is DirectoryInfo)
                {
                    var name = new Markup($"[white{BgColor(i, selectedIndex)}]{item.Name}{Path.DirectorySeparatorChar}[/]");
                    grid.AddRow(new Markup(Emoji.Known.FileFolder), name);
                }
                else
                {
                    var name = new Markup($"[white{BgColor(i, selectedIndex)}]{item.Name}[/]");
                    
                    var renderable = item.Extension switch
                    {
                        ".png" => (IRenderable)new CanvasImage(item.FullName).MaxWidth(5),
                        ".jpg" => new CanvasImage(item.FullName).MaxWidth(5),
                        ".jpeg" => new CanvasImage(item.FullName).MaxWidth(5),
                        ".gif" => new CanvasImage(item.FullName).MaxWidth(5),
                        _ => new Markup(Emoji.Known.PageWithCurl),
                    };
                    grid.AddRow(renderable, name);
                }

                ++i;
            }

            explorerUi.UpdateLeft(grid, title);
        }
    }
}
