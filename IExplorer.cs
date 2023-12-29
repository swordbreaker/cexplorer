public interface IExplorer : IDisposable
{
    DirectoryInfo WorkingDirectory { get; }
    FileSystemInfo SelectedItem { get; }
    int ItemCount { get; }
    int FileIndex { get; set; }
    string Filter { get; set; }
    IEnumerable<FileSystemInfo> Items { get; }
    IEnumerable<FileSystemInfo> AllItems { get; }

    void EnterDirectory(DirectoryInfo directory);
    void ExecuteSelectedItem();
    void GoBackADirectory();
    void RequestDeleteCurrent();
    Task StartAsync();
    void StartFiltering();
    void StartRenameCurrent();
    void ToggleRenderer();
}