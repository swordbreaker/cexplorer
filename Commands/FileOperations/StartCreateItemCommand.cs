using console_explorer.Services;

namespace console_explorer.Commands.FileOperations
{
    public class StartCreateItemCommand(ICreateItemService createItemService) : ICommand
    {
        public string Name => "Create Item";

        public Task ExecuteAsync()
        {
            createItemService.CreateItem();
            return Task.CompletedTask;
        }
    }
}
