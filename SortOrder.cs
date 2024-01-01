namespace console_explorer
{
    public enum SortOrder
    {
        Default,
        NameAscending,
        NameDescending,
        DateModifiedAscending,
        DateModifiedDescending,
        DateCreatedAscending,
        DateCreatedDescending,
        SizeAscending,
        SizeDescending,
    }

    public class SortOderComparer : IComparer<FileSystemInfo>
    {
        private readonly SortOrder sortOrder;

        public SortOderComparer(SortOrder sortOrder)
        {
            this.sortOrder = sortOrder;
        }

        public int Compare(FileSystemInfo? x, FileSystemInfo? y)
        {
            if (x is null)
            {
                return 1;
            }

            if (y is null)
            {
                return -1;
            }

            return sortOrder switch
            {
                SortOrder.Default when x is DirectoryInfo && y is FileInfo => -1,
                SortOrder.Default when x is FileInfo && y is DirectoryInfo => 1,
                SortOrder.NameAscending => x.Name.CompareTo(y.Name),
                SortOrder.NameDescending => y.Name.CompareTo(x.Name),
                SortOrder.DateModifiedAscending => x.LastWriteTime.CompareTo(y.LastWriteTime),
                SortOrder.DateModifiedDescending => y.LastWriteTime.CompareTo(x.LastWriteTime),
                SortOrder.DateCreatedAscending => x.CreationTime.CompareTo(y.CreationTime),
                SortOrder.DateCreatedDescending => y.CreationTime.CompareTo(x.CreationTime),
                SortOrder.SizeAscending when x is FileInfo fx && y is FileInfo fy =>
                    fx.Length.CompareTo(fy.Length),
                SortOrder.SizeDescending when x is FileInfo fx && y is FileInfo fy =>
                    fy.Length.CompareTo(fx.Length),
                SortOrder.SizeAscending when x is DirectoryInfo && y is FileInfo => -1,
                SortOrder.SizeDescending when x is DirectoryInfo && y is FileInfo => 1,
                SortOrder.SizeAscending when x is FileInfo && y is DirectoryInfo => 1,
                SortOrder.SizeDescending when x is FileInfo && y is DirectoryInfo => -1,
                _ => 1,
            };
        }
    }
}
