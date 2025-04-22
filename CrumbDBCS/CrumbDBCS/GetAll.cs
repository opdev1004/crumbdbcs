using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<Dictionary<string, string>> GetAll(string dirname, Encoding? encoding = null)
        {
            try
            {
                Dictionary<string, string> result = [];

                if (!Directory.Exists(dirname)) return result;

                Encoding fileEncoding = encoding ?? Encoding.UTF8;

                string[] filenames = Directory.GetFiles(dirname, "*.json");

                foreach (string filename in filenames)
                {
                    SemaphoreSlim fileLock = GetFileLock(filename);
                    await fileLock.WaitAsync();
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


        public async Task<Dictionary<string, string>> GetAll(string dirname, string databasename, string collectionname, Encoding? encoding = null)
        {
            try
            {
                string documentDirname = Path.Combine(dirname, databasename, collectionname);
                Dictionary<string, string> result = [];

                if (!Directory.Exists(documentDirname)) return result;

                Encoding fileEncoding = encoding ?? Encoding.UTF8;

                string[] filenames = Directory.GetFiles(documentDirname, "*.json");

                foreach (string filename in filenames)
                {
                    SemaphoreSlim fileLock = GetFileLock(filename);
                    await fileLock.WaitAsync();
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
