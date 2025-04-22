using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Remove(string dirname, string documentname)
        {
            string filename = Path.Combine(dirname, $"{documentname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();

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
                fileLock.Release();
            }
        }


        public async Task<bool> Remove(string dirname, string databasename, string collectionname, string documentname)
        {
            string collectionDirname = Path.Combine(dirname, databasename, collectionname);
            string filename = Path.Combine(collectionDirname, $"{documentname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();
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
                fileLock.Release();
            }
        }
    }
}
