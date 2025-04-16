using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<string> Get(string dirname, string keyname, Encoding? encoding=null)
        {
            string filename = Path.Combine(dirname, $"{keyname}.json");
            SemaphoreSlim fileLock = GetFileLock(filename);
            await fileLock.WaitAsync();

            try
            {
                if (!File.Exists(filename)) return "";

                Encoding fileEncoding = encoding ?? Encoding.UTF8;
                string content = await File.ReadAllTextAsync(filename, fileEncoding);
                Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(content) ?? [];

                if (dict.Count == 0) return "";

                return dict.GetValueOrDefault(keyname) ?? "";
            }
            catch (Exception)
            {
                return "";
            }
            finally
            {
                fileLock.Release();
            }
        }

    }
}
