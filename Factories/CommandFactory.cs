using console_explorer.Commands;
using console_explorer.Commands.FileOperations;
using console_explorer.Commands.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace console_explorer.Factories
{
    public class CommandFactory : ICommandFactory
    {
        private readonly IServiceProvider serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public ICommand Create<T>()
            where T : ICommand =>
                serviceProvider.GetRequiredService<T>();

        public ICommand CreateSelectPreviousItemCommand() =>
            new ChangeItemSelectionCommand(serviceProvider.GetRequiredService<IExplorer>(), -1);

        public ICommand CreateSelectNextItemCommand() =>
            new ChangeItemSelectionCommand(serviceProvider.GetRequiredService<IExplorer>(), 1);

        public ICommand CreateDeleteCommand(FileSystemInfo itemToDelete) =>
            new DeleteCommand(itemToDelete);

        public ICommand CreateCreateItemCommand(string name) =>
            new CreateItemCommand(
                name,
                serviceProvider.GetRequiredService<IExplorer>(),
                serviceProvider.GetRequiredService<ILogger<CreateItemCommand>>());
    }
}
