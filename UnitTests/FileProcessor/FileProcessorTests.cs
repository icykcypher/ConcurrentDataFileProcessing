using System.IO;
using System.Collections.Generic;
using ConcurrentDataFileProcessing.src.Domain;
using ConcurrentDataFileProcessing.src.Processing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcurrentDataFileProcessing.src.Infrastructure;

namespace UnitTests.FileProcessing
{
    [TestClass]
    public class FileProcessorTests
    {
        private AppConfig GetFakeConfig()
        {
            return new AppConfig
            {
                InputDirectory = "input",
                ProcessedDirectory = "processed",
                ErrorDirectory = "error",
                Workers = 2
            };
        }

        [TestMethod]
        public void Process_ShouldReturnFalse_WhenFileDoesNotExist()
        {
            var repo = new FakeRepository();
            var aggr = new TemperatureAggregator();
            var config = GetFakeConfig();
            var processor = new FileProcessor(repo, aggr, config);

            var job = new FileProcessingJob { FilePath = "not_exists.json" };

            var result = processor.Process(job);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Process_ShouldInsertParsedData()
        {
            var repo = new FakeRepository();
            var aggr = new TemperatureAggregator();
            var config = GetFakeConfig();
            var processor = new FileProcessor(repo, aggr, config);

            var tempFile = "test.json";
            File.WriteAllText(tempFile, @"[
                { ""number"":0, ""step"":0, ""surface"":0.0,
                  ""valid_time"":1704103200000, ""t2m"":280.4221191406 }
            ]");

            var job = new FileProcessingJob { FilePath = tempFile };
            var result = processor.Process(job);

            Assert.IsTrue(result);
            Assert.AreEqual(1, repo.Inserted.Count);

            File.Delete(tempFile);
        }
    }

    class FakeRepository : IMeasurementRepository
    {
        public readonly List<Measurement> Inserted = new List<Measurement>();

        public void InsertMeasurements(IEnumerable<Measurement> m)
        {
            Inserted.AddRange(m);
        }
    }
}
