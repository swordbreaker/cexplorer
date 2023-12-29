namespace console_explorer.Commands;

public interface ICommand
{
    Task ExecuteAsync();
    string Name { get; }
}