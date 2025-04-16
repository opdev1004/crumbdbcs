using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Remove(string dirname, string keyname)
        {
            string filename = Path.Combine(dirname, $"{keyname}.json");
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
