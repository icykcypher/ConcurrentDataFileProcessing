using System;
using System.IO;
using ConcurrentDataFileProcessing.src.Infrastructure;

namespace ConcurrentDataFileProcessing.src.Processing
{
    /// <summary>
    /// Processes files containing measurement data by parsing, saving to database,
    /// updating temperature aggregation, and moving the files to processed or error folders.
    /// </summary>
    public class FileProcessor
    {
        private readonly SqliteRepository _db;
        private readonly TemperatureAggregator _aggregator;
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileProcessor"/> class.
        /// </summary>
        /// <param name="db">The SQLite repository used for storing measurements.</param>
        /// <param name="aggregator">The aggregator used to compute statistics from measurements.</param>
        public FileProcessor(SqliteRepository db, TemperatureAggregator aggregator)
        {
            _db = db;
            _aggregator = aggregator;
        }

        /// <summary>
        /// Processes a single file: parses its measurements, inserts them into the database,
        /// updates the aggregator, and moves the file to the appropriate output folder.
        /// </summary>
        /// <param name="job">The <see cref="FileProcessingJob"/> containing the file path.</param>
        /// <returns>True if processing succeeded; false otherwise.</returns>
        public bool Process(FileProcessingJob job)
        {
            try
            {
                var data = FileParser.Parse(job.FilePath);
                if (data == null || data.Count == 0)
                {
                    Console.WriteLine("[WARN] No data parsed from file: " + job.FilePath);
                    Move(job.FilePath, "output/error");
                    return false;
                }

                lock (_lock)
                {
                    _db.InsertMeasurements(data);
                }

                _aggregator.AddMeasurements(data);

                Move(job.FilePath, "output/processed");

                Console.WriteLine("[OK] Parsed " + data.Count + " rows from " + job.FilePath);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] " + ex.Message);
                Move(job.FilePath, "output/error");
            }

            return false;
        }

        /// <summary>
        /// Moves a file to a target folder. If a file with the same name exists in the target, it is replaced.
        /// </summary>
        /// <param name="path">The source file path.</param>
        /// <param name="targetFolder">The target folder path.</param>
        private void Move(string path, string targetFolder)
        {
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            var dest = Path.Combine(targetFolder, Path.GetFileName(path));
            if (File.Exists(dest))
                File.Delete(dest);

            File.Move(path, dest);
        }
    }
}