namespace console_explorer.Services
{
    public interface IItemsRenderer
    {
        void RenderItems(IEnumerable<FileSystemInfo> items, int selectedIndex);
    }
}