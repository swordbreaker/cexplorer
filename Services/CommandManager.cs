using console_explorer.Commands;
using console_explorer.Commands.Clipboard;
using console_explorer.Commands.FileOperations;
using console_explorer.Commands.Navigation;
using console_explorer.Commands.UserInterface;
using console_explorer.ExtensionMethods;
using console_explorer.Factories;
using Microsoft.Extensions.Logging;

namespace console_explorer.Services;

public class CommandManager(ICommandFactory commandFactory, ILogger<CommandManager> logger)
    : ICommandManager
{
    private readonly Stack<IUndoableCommand> undoStack = new();
    private readonly Stack<IUndoableCommand> redoStack = new();
    private readonly Dictionary<ConsoleKeyInfo, Func<ICommand>> commands = new()
    {
        [new ConsoleKeyInfo('\0', ConsoleKey.UpArrow, false, false, false)] = commandFactory.CreateSelectPreviousItemCommand,
        [new ConsoleKeyInfo('\0', ConsoleKey.DownArrow, false, false, false)] = commandFactory.CreateSelectNextItemCommand,
        [new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false)] = commandFactory.Create<ExecuteCurrentItemCommand>,
        [new ConsoleKeyInfo('\b', ConsoleKey.Backspace, false, false, false)] = commandFactory.Create<GoBackADirectoryCommand>,
        [new ConsoleKeyInfo('\0', ConsoleKey.Home, false, false, false)] = commandFactory.Create<GoToFirstItemCommand>,
        [new ConsoleKeyInfo('\0', ConsoleKey.End, false, false, false)] = commandFactory.Create<GoToLastItemCommand>,
        [new ConsoleKeyInfo('\u001a', ConsoleKey.Z, false, false, true)] = commandFactory.Create<UndoCommand>,
        [new ConsoleKeyInfo('\u0019', ConsoleKey.Y, false, false, true)] = commandFactory.Create<RedoCommand>,
        [new ConsoleKeyInfo('\0', ConsoleKey.Delete, false, false, false)] = commandFactory.Create<RequestDeletionCommand>,
        [new ConsoleKeyInfo('\t', ConsoleKey.Tab, false, false, false)] = commandFactory.Create<ToggleRendererCommand>,
    };
    private readonly Dictionary<char, Func<ICommand>> charCommands = new()
    {
        ['q'] = commandFactory.Create<QuitCommand>,
        ['c'] = commandFactory.Create<ClipboardCopyCommand>,
        ['p'] = commandFactory.Create<ClipboardPasteCommand>,
        ['x'] = commandFactory.Create<ClipboardCutCommand>,
        ['r'] = commandFactory.Create<StartRenameCommand>,
        ['l'] = commandFactory.Create<CalculateSizeCommand>,
        ['/'] = commandFactory.Create<StartFilterItemsCommand>,
        ['+'] = commandFactory.Create<TogglePreviewPanelSizeCommand>,
        ['?'] = commandFactory.Create<HelpCommand>,
        ['v'] = commandFactory.Create<ClipboardPasteCommand>,
        ['s'] = commandFactory.Create<ToggleSortOrderCommand>,
        ['e'] = commandFactory.Create<OpenExplorerCommand>,
        ['t'] = commandFactory.Create<OpenTerminalCommand>,
        ['n'] = commandFactory.Create<StartCreateItemCommand>,
        ['R'] = commandFactory.Create<RunCommand>,
    };

    public event Action<ICommand>? OnCommandExecuted;

    public IEnumerable<(string key, string commandName)> GetCommandHelp =>
        commands
            .Select(x => (x.Key.ToDisplayString(), x.Value().Name))
            .Concat(charCommands
                .Select(x => (x.Key.ToString(), x.Value().Name)));

    public async Task Execute(ConsoleKeyInfo key)
    {
        if (commands.TryGetValue(key, out var command))
        {
            await Execute(command());
        }
        else if (charCommands.TryGetValue(key.KeyChar, out var charCommand))
        {
            await Execute(charCommand());
        }
    }

    public async Task Execute<T>() where T : ICommand
    {
        try
        {
            await Execute(commandFactory.Create<T>());
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    public async Task Execute(ICommand command)
    {
        try
        {
            await command.ExecuteAsync();
            if (command is IUndoableCommand undoableCommand)
            {
                undoStack.Push(undoableCommand);
            }
            redoStack.Clear();
            OnCommandExecuted?.Invoke(command);
        }
        catch(Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    public async Task Undo()
    {
        try
        {
            if (undoStack.Count == 0)
            {
                return;
            }

            var command = undoStack.Pop();
            await command.UndoAsync();
            redoStack.Push(command);
        }
        catch(Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    public async Task Redo()
    {
        try
        {
            if (redoStack.Count == 0)
            {
                return;
            }

            var command = redoStack.Pop();
            await command.ExecuteAsync();
            undoStack.Push(command);
        }
        catch(Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }
}
