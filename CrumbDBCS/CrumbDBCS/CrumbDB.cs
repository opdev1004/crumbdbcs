namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        private static readonly Dictionary<string, SemaphoreSlim> _fileLocks = new();
        private static readonly object _fileLocksLock = new();
    }
}
