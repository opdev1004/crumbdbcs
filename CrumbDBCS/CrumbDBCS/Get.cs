using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<string> Get(string dirname, string databasename, string collectionname, string documentname, Encoding? encoding = null)
        {
            string collectionDirname = Path.Combine(dirname, databasename, collectionname);
            string filename = Path.Combine(collectionDirname, $"{documentname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();
			await IOSemaphore.WaitAsync();

			try
            {
                if (!File.Exists(filename)) return "";

                Encoding fileEncoding = encoding ?? Encoding.UTF8;
                string content = await File.ReadAllTextAsync(filename, fileEncoding);
                return content;
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                IOSemaphore.Release();
                fileLock.Release();
            }
        }
    }
}
