using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task Insert(string dirname, string keyname, string value, Encoding? encoding=null)
        {
            await _semaphore.WaitAsync();
            try
            {
                Directory.CreateDirectory(dirname);
                string filename = Path.Combine(dirname, $"{keyname}.json");
                Encoding fileEncoding = encoding ?? Encoding.UTF8;
                string json = JsonSerializer.Serialize(new Dictionary<string, string> { { keyname, value } });
                await File.WriteAllTextAsync(filename, json, fileEncoding);
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}
