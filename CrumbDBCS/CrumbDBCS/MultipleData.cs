namespace CrumbDBCS
{
    public partial class CrumbDB
    {
		public class MultipleData
		{
			public Dictionary<string, string> Data { get; set; } = [];
			public int DBNextPosition { get; set; } = 0;
			public bool DBEnd { get; set; } = false;
		}
	}
}
