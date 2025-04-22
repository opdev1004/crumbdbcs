using CrumbDBCS;
using System.Text;

[TestFixture]
public class CrumbDBTests2
{
    private string _testDir = Path.Combine(Path.GetTempPath(), "CrumbDB_Test");
    private CrumbDB _db;

    [SetUp]
    public void Setup()
    {
        if (Directory.Exists(_testDir)) Directory.Delete(_testDir, true);
        Directory.CreateDirectory(_testDir);
        _db = new CrumbDB();
    }

    [TearDown]
    public void Cleanup()
    {
        if (Directory.Exists(_testDir)) Directory.Delete(_testDir, true);
    }

    // Existing flat structure tests...

    [Test]
    public async Task Insert_And_Get_Nested_ReturnsInsertedValue()
    {
        await _db.Insert(_testDir, "mydb", "mycollection", "testkey", "testvalue");
        string result = await _db.Get(Path.Combine(_testDir, "mydb", "mycollection"), "testkey");
        Assert.That(result, Is.EqualTo("testvalue"));
    }

    [Test]
    public async Task Remove_Nested_ReturnsTrue()
    {
        await _db.Insert(_testDir, "mydb", "mycollection", "toremove", "value");
        bool removed = await _db.Remove(_testDir, "mydb", "mycollection", "toremove");
        Assert.That(removed, Is.True);
    }

    [Test]
    public async Task GetAll_Nested_ReturnsAllKeyValuePairs()
    {
        await _db.Insert(_testDir, "mydb", "mycollection", "key1", "val1");
        await _db.Insert(_testDir, "mydb", "mycollection", "key2", "val2");
        var result = await _db.GetAll(_testDir, "mydb", "mycollection", Encoding.UTF8);

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result["key1"], Is.EqualTo("val1"));
        Assert.That(result["key2"], Is.EqualTo("val2"));
    }

    [Test]
    public async Task GetMultiple_Nested_ReturnsCorrectSubset()
    {
        for (int i = 0; i < 5; i++)
        {
            await _db.Insert(_testDir, "mydb", "mycollection", $"key{i}", $"val{i}");
        }

        var result = await _db.GetMultiple(_testDir, "mydb", "mycollection", 1, 3, Encoding.UTF8);

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.ContainsKey("key1"));
        Assert.That(result.ContainsKey("key2"));
        Assert.That(result.ContainsKey("key3"));
    }
}
