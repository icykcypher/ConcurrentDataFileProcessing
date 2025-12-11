using ConcurrentDataFileProcessing.src.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace UnitTests.Watcher
{
    [TestClass]
    public class FileWatcherServiceTests
    {
        private string _dir;

        [TestInitialize]
        public void Setup()
        {
            _dir = Path.Combine(Path.GetTempPath(), "watcher_test");
            if (Directory.Exists(_dir))
                Directory.Delete(_dir, true);
            Directory.CreateDirectory(_dir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_dir))
                Directory.Delete(_dir, true);
        }

        [TestMethod]
        public void Watcher_ShouldAddFileToQueue()
        {
            var queue = new BlockingCollection<FileProcessingJob>();
            var watcher = new FileWatcherService(_dir, queue, "processed", "error");

            watcher.Start();

            var file = Path.Combine(_dir, "data.json");
            File.WriteAllText(file, "{}");

            Thread.Sleep(300); 

            watcher.Stop();

            Assert.IsTrue(queue.Count == 1);
        }
    }
}
