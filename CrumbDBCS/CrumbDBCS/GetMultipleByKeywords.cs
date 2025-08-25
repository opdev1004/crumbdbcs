using System.Text;
using System.Text.Json;
using System.Threading;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<MultipleData> GetMultipleByKeywords(string dirname, string databasename, string collectionname, List<string> keywords, int position, int count, Encoding? encoding = null)
        {
			MultipleData result = new();

            if (position < 0 || count <= 0) return result;

			try
            {
                string collectionDirname = Path.Combine(dirname, databasename, collectionname);

                if (!Directory.Exists(collectionDirname)) return result;

                Encoding fileEncoding = encoding ?? Encoding.UTF8;

                IEnumerator<string> enumerator = Directory.EnumerateFiles(collectionDirname, "*.json").GetEnumerator();

				List<string> filenames = new(count);
				int seen = 0;
				int taken = 0;
				bool dbEnd = true;

				await IOSemaphore.WaitAsync();
				try
				{
					while (enumerator.MoveNext())
					{
						if (seen++ < position) continue;

						string filename = enumerator.Current;
						string basename = Path.GetFileNameWithoutExtension(filename);

						if (!keywords.Any(k => !string.IsNullOrEmpty(k) && basename.Contains(k, StringComparison.OrdinalIgnoreCase))) continue;

						filenames.Add(filename);

						if (++taken >= count)
						{
							dbEnd = !enumerator.MoveNext();
							break;
						}
					}
				}
				finally
				{
					enumerator.Dispose();
					IOSemaphore.Release();
				}

				foreach (string filename in filenames)
				{
					SemaphoreSlim fileLock = GetFileLock(filename);
					await fileLock.WaitAsync();
					await IOSemaphore.WaitAsync();

					try
					{
						string content = await File.ReadAllTextAsync(filename, fileEncoding);
						string documentname = Path.GetFileNameWithoutExtension(filename);
						result.Data[documentname] = content;
					}
					catch (Exception)
					{
						string documentname = Path.GetFileNameWithoutExtension(filename);
						result.Data[documentname] = "";
					}
					finally
					{
						IOSemaphore.Release();
						fileLock.Release();
					}
				}

				result.DBEnd = dbEnd;
				result.DBNextPosition = seen;

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
    }
}
