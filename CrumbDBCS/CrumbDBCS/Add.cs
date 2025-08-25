using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Add(string dirname, string databasename, string collectionname, string documentname, string value, Encoding? encoding = null)
        {
            string collectionDirname = Path.Combine(dirname, databasename, collectionname);
            string filename = Path.Combine(collectionDirname, $"{documentname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();
			await IOSemaphore.WaitAsync();

			try
            {
                Directory.CreateDirectory(collectionDirname);

                if (File.Exists(filename)) return false;

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
				IOSemaphore.Release();
				fileLock.Release();
            }
        }

    }
}
