using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<Dictionary<string, string>> GetMultiple(string dirname, int position, int count, Encoding? encoding = null)
        {
            try
            {
                Dictionary<string, string> result = [];

                if (!Directory.Exists(dirname)) return result;

                Encoding fileEncoding = encoding ?? Encoding.UTF8;

                IEnumerable<string> filenames = Directory.GetFiles(dirname, "*.json")
                                                     .OrderBy(f => f)
                                                     .Skip(position)
                                                     .Take(count);

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


        public async Task<Dictionary<string, string>> GetMultiple(string dirname, string databasename, string collectionname, int position, int count, Encoding? encoding = null)
        {
            try
            {
                string collectionDirname = Path.Combine(dirname, databasename, collectionname);
                Dictionary<string, string> result = [];

                if (!Directory.Exists(collectionDirname)) return result;

                Encoding fileEncoding = encoding ?? Encoding.UTF8;

                IEnumerable<string> filenames = Directory.GetFiles(collectionDirname, "*.json")
                                                     .OrderBy(f => f)
                                                     .Skip(position)
                                                     .Take(count);

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
