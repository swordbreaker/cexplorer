namespace console_explorer.Services
{
    public interface IFilterService
    {
        bool IsFiltering { get; }

        void Start();
    }
}