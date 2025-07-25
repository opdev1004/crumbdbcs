namespace CrumbDBCS
{
    public partial class CrumbDB
    {
        private Dictionary<string, SemaphoreSlim> _fileLocks = new();
        private object _fileLocksLock = new();
		private SemaphoreSlim IOSemaphore;

		public CrumbDB(int iomax = 512)
        {
			if (iomax < 1) iomax = 512;
			IOSemaphore = new SemaphoreSlim(iomax, iomax);
		}
	}
}
