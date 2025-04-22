using CrumbDBCS;
using System.Text;

namespace UnitTest
{
    [TestFixture]
    public partial class CrumbDBTests
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

        [Test]
        public async Task Insert_And_Get_ReturnsInsertedValue()
        {
            await _db.Insert(_testDir, "testkey", "testvalue");
            string result = await _db.Get(_testDir, "testkey");
            Assert.That("testvalue".Equals(result), Is.True);
        }

        [Test]
        public async Task Get_NonExistent_ReturnsEmptyString()
        {
            string result = await _db.Get(_testDir, "doesnotexist");
            Assert.That("".Equals(result), Is.True);
        }

        [Test]
        public async Task Remove_ExistingFile_ReturnsTrue()
        {
            await _db.Insert(_testDir, "toremove", "value");
            bool removed = await _db.Remove(_testDir, "toremove");
            Assert.That(removed, Is.True);
        }

        [Test]
        public async Task Remove_NonExistentFile_ReturnsFalse()
        {
            bool removed = await _db.Remove(_testDir, "fake");
            Assert.That(removed, Is.False);
        }

        [Test]
        public async Task GetAll_ReturnsAllKeyValuePairs()
        {
            await _db.Insert(_testDir, "key1", "val1");
            await _db.Insert(_testDir, "key2", "val2");
            Dictionary<string, string> result = await _db.GetAll(_testDir, Encoding.UTF8);

            Assert.That(2 == result.Count, Is.True);
            Assert.That("val1".Equals(result["key1"]), Is.True);
            Assert.That("val2".Equals(result["key2"]), Is.True);
        }

        [Test]
        public async Task GetMultiple_ReturnsCorrectSubset()
        {
            for (int i = 0; i < 5; i++)
            {
                await _db.Insert(_testDir, $"key{i}", $"val{i}");
            }

            Dictionary<string, string> result = await _db.GetMultiple(_testDir, 1, 3, Encoding.UTF8);

            Assert.That(3 == result.Count, Is.True);
            Assert.That(result.ContainsKey("key1"), Is.True);
            Assert.That(result.ContainsKey("key2"), Is.True);
            Assert.That(result.ContainsKey("key3"), Is.True);
        }
    }
}
