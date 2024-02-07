namespace console_explorer.Commands.UserInterface
{
    public class ToggleSortOrderCommand : IUndoableCommand
    {
        private readonly IExplorer explorer;
        private SortOrder oldSortOder;

        public string Name => "Toggle Sort Order";

        public ToggleSortOrderCommand(IExplorer explorer)
        {
            this.explorer = explorer;
        }

        public Task ExecuteAsync()
        {
            oldSortOder = explorer.SortOrder;
            explorer.SortOrder = explorer.SortOrder switch
            {
                SortOrder.Default => SortOrder.NameAscending,
                SortOrder.NameAscending => SortOrder.NameDescending,
                SortOrder.NameDescending => SortOrder.DateModifiedAscending,
                SortOrder.DateModifiedAscending => SortOrder.DateModifiedDescending,
                SortOrder.DateModifiedDescending => SortOrder.DateCreatedAscending,
                SortOrder.DateCreatedAscending => SortOrder.DateCreatedDescending,
                SortOrder.DateCreatedDescending => SortOrder.SizeAscending,
                SortOrder.SizeAscending => SortOrder.SizeDescending,
                SortOrder.SizeDescending => SortOrder.Default,
                _ => SortOrder.Default,
            };
            return Task.CompletedTask;
        }

        public Task UndoAsync()
        {
            explorer.SortOrder = oldSortOder;
            return Task.CompletedTask;
        }
    }
}
