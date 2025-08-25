using CrumbDBCS;
using System.IO.Compression;
using System.Text;

namespace UnitTest
{

    [TestFixture]
    public class CrumbDBTests
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
        public async Task Insert_And_Get_Nested_ReturnsInsertedValue()
        {
            await _db.Add(_testDir, "mydb", "mycollection", "testkey", "testvalue");
            string result = await _db.Get(_testDir, "mydb", "mycollection", "testkey");
            Assert.That(result, Is.EqualTo("testvalue"));
        }

        [Test]
        public async Task Remove_Nested_ReturnsTrue()
        {
            await _db.Add(_testDir, "mydb", "mycollection", "toremove", "value");
            bool removed = await _db.Remove(_testDir, "mydb", "mycollection", "toremove");
            Assert.That(removed, Is.True);
        }

        [Test]
        public async Task GetAll_Nested_ReturnsAllKeyValuePairs()
        {
            await _db.Add(_testDir, "mydb", "mycollection", "key1", "val1");
            await _db.Add(_testDir, "mydb", "mycollection", "key2", "val2");
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
                await _db.Add(_testDir, "mydb", "mycollection", $"key{i}", $"val{i}");
            }

            var result = await _db.GetMultiple(_testDir, "mydb", "mycollection", 1, 3, Encoding.UTF8);

            Assert.That(result.Data.Count, Is.EqualTo(3));
            Assert.That(result.Data.ContainsKey("key1"));
            Assert.That(result.Data.ContainsKey("key2"));
            Assert.That(result.Data.ContainsKey("key3"));
        }

		[Test]
		public async Task GetMultipleByKeyword_ReturnsMatchingFiles()
		{
			// Arrange
			await _db.Add(_testDir, "mydb", "mycollection", "apple_doc", "v1");
			await _db.Add(_testDir, "mydb", "mycollection", "banana_doc", "v2");
			await _db.Add(_testDir, "mydb", "mycollection", "apple_banana", "v3");
			await _db.Add(_testDir, "mydb", "mycollection", "cherry", "v4");

			// Act (search by single keyword "apple")
			var result = await _db.GetMultipleByKeyword(_testDir, "mydb", "mycollection", "apple", 0, 10, Encoding.UTF8);

			// Assert
			Assert.That(result.Data.Count, Is.EqualTo(2), "Should find two files containing 'apple'");
			Assert.That(result.Data.ContainsKey("apple_doc"));
			Assert.That(result.Data.ContainsKey("apple_banana"));
		}

		[Test]
		public async Task GetMultipleByKeywords_ReturnsAnyMatchingFiles()
		{
			// Arrange
			await _db.Add(_testDir, "mydb", "mycollection", "foo-Apple", "a");   // mixed-case on purpose
			await _db.Add(_testDir, "mydb", "mycollection", "bar-banana", "b");
			await _db.Add(_testDir, "mydb", "mycollection", "baz-band", "c");
			await _db.Add(_testDir, "mydb", "mycollection", "cherry", "d");
			await _db.Add(_testDir, "mydb", "mycollection", "other", "e");

			var keywords = new List<string> { "ban", "cherry" };

			// Act (should match bar-banana, baz-band, cherry)
			var result = await _db.GetMultipleByKeywords(_testDir, "mydb", "mycollection", keywords, 0, 10, Encoding.UTF8);

			// Assert
			Assert.That(result.Data.Count, Is.EqualTo(3), "Should match any filename containing 'ban' or 'cherry' (case-insensitive)");
			Assert.That(result.Data.ContainsKey("bar-banana"));
			Assert.That(result.Data.ContainsKey("baz-band"));
			Assert.That(result.Data.ContainsKey("cherry"));
		}

		[Test]
		public async Task GetMultipleByKeywords_RespectsCountLimit()
		{
			// Arrange - create 5 matches but request count = 2
			await _db.Add(_testDir, "mydb", "mycollection", "match-1", "v1");
			await _db.Add(_testDir, "mydb", "mycollection", "match-2", "v2");
			await _db.Add(_testDir, "mydb", "mycollection", "match-3", "v3");
			await _db.Add(_testDir, "mydb", "mycollection", "match-4", "v4");
			await _db.Add(_testDir, "mydb", "mycollection", "unrelated", "v5");

			var keywords = new List<string> { "match" };

			// Act (count = 2)
			var result = await _db.GetMultipleByKeywords(_testDir, "mydb", "mycollection", keywords, 0, 2, Encoding.UTF8);

			// Assert
			Assert.That(result.Data.Count, Is.EqualTo(2), "Should only return at most 'count' matching files");
			Assert.That(result.Data.Keys.All(k => k.StartsWith("match")), "Returned keys should be the matching ones");
		}



		[Test]
        public async Task Backup_CreatesZipWithAllFiles()
        {
            // Arrange
            await _db.Add(_testDir, "mydb", "mycollection", "doc1", "value1");
            await _db.Add(_testDir, "mydb", "mycollection", "doc2", "value2");

            string zipPath = Path.Combine(_testDir, "backup.zip");

            // Act
            bool success = await _db.Backup(_testDir, zipPath);

            // Assert
            Assert.That(success, Is.True);
            Assert.That(File.Exists(zipPath), Is.True);

            // Verify contents
            using ZipArchive archive = ZipFile.OpenRead(zipPath);
            var entryNames = archive.Entries.Select(e => e.FullName).ToList();

            Assert.That(entryNames, Does.Contain(Path.Combine("mydb", "mycollection", "doc1.json")));
            Assert.That(entryNames, Does.Contain(Path.Combine("mydb", "mycollection", "doc2.json")));
        }



    }

}