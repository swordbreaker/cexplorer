namespace console_explorer.Services
{
    public interface IRenameService
    {
        bool IsRenaming { get; }

        void Rename(FileSystemInfo selectedItem);
    }
}