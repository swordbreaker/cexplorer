namespace console_explorer.ExtensionMethods;

public static class DirectorySizeCalculator
{
    public static Bytes CalculateSize(this DirectoryInfo dir, IProgress<float> progress)
    {
        var files = dir.GetFiles("*", SearchOption.AllDirectories);
        var count = 0;
        var totalSize = files.Sum(file =>
        {
            count++;
            if (count == 1 || count % 100 == 0 || count == files.Length)
            {
                progress.Report((float)count / files.Length);
            }
            return file.Length;
        });

        return totalSize;
    }
}