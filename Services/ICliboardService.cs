public interface ICliboardService
{
    Task CopyAsync(FileSystemInfo selectedItem);
    Task<(string[] PastedItems, string[] CuttedForm)> PasteAsync(DirectoryInfo workingDirectory);
    Task RegisterForCut(FileSystemInfo selectedItem);
}