using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<Dictionary<string, string>> GetAll(string dirname, string databasename, string collectionname, Encoding? encoding = null)
        {
            try
            {
                string collectionDirname = Path.Combine(dirname, databasename, collectionname);
                Dictionary<string, string> result = [];

                if (!Directory.Exists(collectionDirname)) return result;

                Encoding fileEncoding = encoding ?? Encoding.UTF8;

                string[] filenames = Directory.GetFiles(collectionDirname, "*.json");

                foreach (string filename in filenames)
                {
                    SemaphoreSlim fileLock = GetFileLock(filename);
                    await fileLock.WaitAsync();
					await IOSemaphore.WaitAsync();

					try
                    {
                        string content = await File.ReadAllTextAsync(filename, fileEncoding);
                        string documentname = Path.GetFileNameWithoutExtension(filename);
                        result[documentname] = content;
                    }
                    catch (Exception)
                    {
                        string documentname = Path.GetFileNameWithoutExtension(filename);
                        result[documentname] = "";
                    }
                    finally
                    {
                        IOSemaphore.Release();
                        fileLock.Release();
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
