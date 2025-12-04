using System;
using Serilog;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ConcurrentDataFileProcessing.src.Processing;
using ConcurrentDataFileProcessing.src.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace ConcurrentFileProcessor
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Concurrent File Processor (.NET Framework 4.8)");
            Console.WriteLine("----------------------------------------------");

            InitSerilog();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build()
                .Get<AppConfig>();

            var inputDir = EnsureInputDirectory(config.InputDirectory);
            var queue = new BlockingCollection<FileProcessingJob>();
            var db = new SqliteRepository(config.Database.Path);
            var watcher = new FileWatcherService(inputDir, queue, config.ProcessedDirectory, config.ErrorDirectory);
            var aggregator = new TemperatureAggregator();
            var processor = new FileProcessor(db, aggregator);

            watcher.Start();

            StartWorkers(queue, processor, watcher, workerCount: config.Workers);

            Console.WriteLine("Watching folder: " + inputDir);
            Console.WriteLine("Drop JSON files to input directory");
            Console.WriteLine("Press ENTER to print results and exit.");
            Console.ReadLine();

            queue.CompleteAdding();
            watcher.Stop();

            aggregator.PrintMonthlyAndYearlyAverage();
            Console.WriteLine("Shutting down...");
        }

        private static string EnsureInputDirectory(string name)
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), name);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        private static void InitSerilog()
        {
            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()         
              .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
              .CreateLogger();
        }

        private static void StartWorkers(
            BlockingCollection<FileProcessingJob> queue,
            FileProcessor processor,
            FileWatcherService watcher,
            int workerCount)
        {
            for (int i = 0; i < workerCount; i++)
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
        }
    }
}