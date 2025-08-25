using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Remove(string dirname, string databasename, string collectionname, string documentname)
        {
            string collectionDirname = Path.Combine(dirname, databasename, collectionname);
            string filename = Path.Combine(collectionDirname, $"{documentname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();
			await IOSemaphore.WaitAsync();

			try
            {
                if (!File.Exists(filename)) return false;
                File.Delete(filename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                IOSemaphore.Release();
                fileLock.Release();
            }
        }
    }
}
