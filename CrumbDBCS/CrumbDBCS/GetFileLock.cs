using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        private SemaphoreSlim GetFileLock(string filename)
        {
            lock (_fileLocksLock)
            {
                if (!_fileLocks.TryGetValue(filename, out var sem))
                {
                    sem = new SemaphoreSlim(1, 1);
                    _fileLocks[filename] = sem;
                }
                return sem;
            }
        }
    }
}
