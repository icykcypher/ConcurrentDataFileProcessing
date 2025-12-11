using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcurrentDataFileProcessing.src.Infrastructure;
using ConcurrentDataFileProcessing.src.Domain;
using System.Collections.Generic;

namespace UnitTests.Repository
{
    [TestClass]
    public class SqliteRepositoryTests
    {
        [TestMethod]
        public void InsertMeasurements_ShouldInsertData()
        {
            var repo = new SqliteRepository(":memory:");
            repo.Init();

            repo.InsertMeasurements(new[]
            {
                new Measurement
                {
                    Step = 0,
                    Surface = 0,
                    Temperature2m = 10,
                    Timestamp = System.DateTime.UtcNow
                }
            });

            var items = repo.QueryAll();

            Assert.AreEqual(1, new List<Measurement>(items).Count);
        }
    }
}
