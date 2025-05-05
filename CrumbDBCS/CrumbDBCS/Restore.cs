using System.Text.Json;
using System.Text;
using System.IO.Compression;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Restore(string zipPath, string destDir)
        {
            try
            {
                using FileStream zipStream = new(zipPath, FileMode.Open);
                using ZipArchive archive = new(zipStream, ZipArchiveMode.Read);

                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string destPath = Path.Combine(destDir, entry.FullName);

                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(destPath);
                        continue;
                    }

                    SemaphoreSlim fileLock = GetFileLock(destPath);
                    await fileLock.WaitAsync();

                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

                        using Stream entryStream = entry.Open();
                        using FileStream fileStream = new(destPath, FileMode.Create, FileAccess.Write);
                        await entryStream.CopyToAsync(fileStream);
                    }
                    finally
                    {
                        fileLock.Release();
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
