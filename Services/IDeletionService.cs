namespace console_explorer.Services
{
    public interface IDeletionService
    {
        void CancelDeletion();
        void Delete();
        void RequestDeletion(FileSystemInfo item);
    }
}