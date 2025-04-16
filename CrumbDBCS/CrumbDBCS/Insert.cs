using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Insert(string dirname, string keyname, string value, Encoding? encoding=null)
        {
            Directory.CreateDirectory(dirname);
            string filename = Path.Combine(dirname, $"{keyname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();

            try
            {
                Encoding fileEncoding = encoding ?? Encoding.UTF8;
                string json = JsonSerializer.Serialize(new Dictionary<string, string> { { keyname, value } });
                await File.WriteAllTextAsync(filename, json, fileEncoding);
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
