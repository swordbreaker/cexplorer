public interface ICliboardService
{
    Task CopyAsync(FileSystemInfo selectedItem);
    Task PasteAsync(DirectoryInfo workingDirectory);
}