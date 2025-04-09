using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<Dictionary<string, string>> GetMultiple(string dirname, int position, int count, Encoding encoding)
        {
            await _semaphore.WaitAsync();
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
                    string content = await File.ReadAllTextAsync(filename, fileEncoding);
                    Dictionary<string, string> dict = JsonSerializer.Deserialize<Dictionary<string, string>>(content) ?? [];

                    if (dict.Count == 0)
                    {
                        string keyname = Path.GetFileNameWithoutExtension(filename);
                        result[keyname] = "";
                    }
                    else
                    {
                        foreach (KeyValuePair<string, string> kvp in dict)
                            result[kvp.Key] = kvp.Value;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return [];
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}
