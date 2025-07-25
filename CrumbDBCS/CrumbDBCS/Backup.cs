using System.IO.Compression;

namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        public async Task<bool> Backup(string sourceDir, string zipPath)
        {
            try
            {
                if (File.Exists(zipPath)) File.Delete(zipPath);

                using FileStream zipToOpen = new(zipPath, FileMode.CreateNew);
                using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Create);

                foreach (string filepath in Directory.EnumerateFiles(sourceDir, "*.json", SearchOption.AllDirectories))
                {
                    SemaphoreSlim fileLock = GetFileLock(filepath);
                    await fileLock.WaitAsync();
					await IOSemaphore.WaitAsync();

					try
                    {
                        string relativePath = Path.GetRelativePath(sourceDir, filepath);
                        string fileContent = await File.ReadAllTextAsync(filepath);

                        ZipArchiveEntry entry = archive.CreateEntry(relativePath);
                        using StreamWriter writer = new(entry.Open());
                        await writer.WriteAsync(fileContent);
                    }
                    finally
                    {
                        IOSemaphore.Release();
                        fileLock.Release();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
