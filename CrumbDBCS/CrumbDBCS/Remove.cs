using System.Text.Json;
using System.Text;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Remove(string dirname, string keyname)
        {
            await _semaphore.WaitAsync();
            try
            {
                string filename = Path.Combine(dirname, $"{keyname}.json");

                if (!File.Exists(filename)) return false;

                File.Delete(filename);
                return true;
            }
            finally
            {
                _semaphore.Release();
            }
        }

    }
}
