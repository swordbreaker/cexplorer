using console_explorer.Commands;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
