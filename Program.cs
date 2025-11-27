using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ConcurrentDataFileProcessing.src.Processing;
using ConcurrentDataFileProcessing.src.Infrastructure;

namespace ConcurrentFileProcessor
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Concurrent File Processor (.NET Framework 4.8)");
            Console.WriteLine("----------------------------------------------");

            var inputDir = Path.Combine(Directory.GetCurrentDirectory(), "input");
            if (!Directory.Exists(inputDir))
                Directory.CreateDirectory(inputDir);

            var queue = new BlockingCollection<FileProcessingJob>();
            var db = new SqliteRepository("Data Source=data.db");
            db.Init();

            var aggregator = new TemperatureAggregator();
            var processor = new FileProcessor(db, aggregator);

            var watcher = new FileWatcherService(inputDir, queue, "processed", "error");
            watcher.Start();

            for (int i = 0; i < 4; i++)
            {
                int workerId = i;
                Console.WriteLine($"Starting Worker {workerId}");

                Task.Factory.StartNew(() =>
                {
                    foreach (var job in queue.GetConsumingEnumerable())
                    {
                        try
                        {
                            if (!File.Exists(job.FilePath))
                            {
                                Console.WriteLine($"[WARN][Worker {workerId}] File not found: {job.FilePath}");
                                continue;
                            }

                            Console.WriteLine($"[PROCESS][Worker {workerId}] {job.FilePath}");
                            bool success = processor.Process(job);

                            if (success)
                                watcher.MoveToProcessed(job.FilePath);
                            else
                                watcher.MoveToError(job.FilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR][Worker {workerId}] {ex.Message}");
                            watcher.MoveToError(job.FilePath);
                        }
                    }
                }, TaskCreationOptions.LongRunning);
            }

            Console.WriteLine("Watching folder: " + inputDir);
            Console.WriteLine("Drop CSV / JSON files to input directory");
            Console.WriteLine("Press ENTER to exit.");
            Console.ReadLine();

            queue.CompleteAdding();
            watcher.Stop();

            aggregator.PrintMonthlyAndYearlyAverage();

            Console.WriteLine("Shutting down...");
        }
    }
}