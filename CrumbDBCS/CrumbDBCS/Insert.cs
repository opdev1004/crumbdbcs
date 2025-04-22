using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Insert(string dirname, string documentname, string value, Encoding? encoding=null)
        {
            string filename = Path.Combine(dirname, $"{documentname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();

            try
            {
                Directory.CreateDirectory(dirname);
                Encoding fileEncoding = encoding ?? Encoding.UTF8;
                await File.WriteAllTextAsync(filename, value, fileEncoding);
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

        public async Task<bool> Insert(string dirname, string databasename, string collectionname, string documentname, string value, Encoding? encoding = null)
        {
            string collectionDirname = Path.Combine(dirname, databasename, collectionname);
            string filename = Path.Combine(collectionDirname, $"{documentname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();

            try
            {
                Directory.CreateDirectory(collectionDirname);
                Encoding fileEncoding = encoding ?? Encoding.UTF8;
                await File.WriteAllTextAsync(filename, value, fileEncoding);
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
