namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
    }
}
