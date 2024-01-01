using Spectre.Console;

namespace console_explorer.Services
{
    public abstract class ItemsRenderer : IItemsRenderer
    {
        public abstract void RenderItems(IEnumerable<FileSystemInfo> items, int selectedIndex, string title);

        protected string BgColor(int i, int index) => i == index ? " on blue" : string.Empty;
        protected Markup GetMarkup(string text, int i, int index) =>
            i == index ? new Markup($"[white on blue]{text}[/]") : new Markup(text);
    }
}
