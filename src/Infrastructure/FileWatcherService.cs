using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace ConcurrentDataFileProcessing.src.Infrastructure
{
    /// <summary>
    /// Represents a single file processing job containing the path to the file.
    /// </summary>
    public class FileProcessingJob
    {
        /// <summary>
        /// Full path of the file to be processed.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Watches a directory for new files, queues them for processing, and moves them to
    /// processed or error folders after processing.
    /// </summary>
    public class FileWatcherService
    {
        private readonly string _input;
        private readonly string _processed;
        private readonly string _error;
        private readonly BlockingCollection<FileProcessingJob> _queue;
        private FileSystemWatcher _watcher;
        private volatile bool _running = false;

        private readonly HashSet<string> _seenFiles = new HashSet<string>();
        private readonly ConcurrentDictionary<string, bool> _processingFiles = new ConcurrentDictionary<string, bool>();
        private readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWatcherService"/> class.
        /// </summary>
        /// <param name="input">The directory to watch for new files.</param>
        /// <param name="queue">The queue to enqueue files for processing.</param>
        /// <param name="processed">The directory to move successfully processed files.</param>
        /// <param name="error">The directory to move files that failed processing.</param>
        public FileWatcherService(string input, BlockingCollection<FileProcessingJob> queue, string processed, string error)
        {
            _input = input;
            _queue = queue;
            _processed = processed;
            _error = error;

            Directory.CreateDirectory(_processed);
            Directory.CreateDirectory(_error);
        }

        /// <summary>
        /// Starts watching the input directory for new files.
        /// </summary>
        public void Start()
        {
            _running = true;

            _watcher = new FileSystemWatcher(_input)
            {
                Filter = "*.*",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };
            _watcher.Created += OnCreated;

            foreach (var file in Directory.GetFiles(_input))
            {
                EnqueueFile(file);
            }

            Console.WriteLine($"[WATCHER] Started on {_input}");
        }

        /// <summary>
        /// Stops watching the directory and completes the processing queue.
        /// </summary>
        public void Stop()
        {
            _running = false;
            if (_watcher != null)
            {
                _watcher.Created -= OnCreated;
                _watcher.Dispose();
            }
            _queue.CompleteAdding();
        }


        /// <summary>
        /// Handles the event when a new file is created in the watched directory.
        /// </summary>
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            Task.Run(() =>
            {
                int attempts = 5;
                while (attempts-- > 0)
                {
                    if (IsFileReady(e.FullPath))
                    {
                        EnqueueFile(e.FullPath);
                        break;
                    }
                    Thread.Sleep(200);
                }
            });
        }

        /// <summary>
        /// Enqueues a file for processing if it has not been seen or processed already.
        /// </summary>
        /// <param name="path">The file path to enqueue.</param>
        private void EnqueueFile(string path)
        {
            try
            {
                lock (_lock)
                {
                    if (_seenFiles.Contains(path)) return;
                    _seenFiles.Add(path);

                    if (!_processingFiles.TryAdd(path, true))
                        return;
                }

                if (File.Exists(path) && IsFileReady(path))
                {
                    _queue.Add(new FileProcessingJob { FilePath = path });
                    Console.WriteLine("[QUEUE] New file: " + path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Could not enqueue file: " + path);
                Console.WriteLine(ex.Message);
                MoveToError(path);
            }
        }

        /// <summary>
        /// Moves a successfully processed file to the processed directory.
        /// </summary>
        /// <param name="path">The file path to move.</param>
        public void MoveToProcessed(string path)
        {
            try
            {
                if (!File.Exists(path)) return;

                var dest = Path.Combine(_processed, Path.GetFileName(path));
                File.Move(path, dest);

                _processingFiles.TryRemove(path, out _);
                lock (_lock)
                {
                    _seenFiles.Remove(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] MoveToProcessed failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Moves a file that failed processing to the error directory.
        /// </summary>
        /// <param name="path">The file path to move.</param>
        public void MoveToError(string path)
        {
            try
            {
                if (!File.Exists(path)) return;

                var dest = Path.Combine(_error, Path.GetFileName(path));
                File.Move(path, dest);

                _processingFiles.TryRemove(path, out _);
                lock (_lock)
                {
                    _seenFiles.Remove(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] MoveToError failed: " + ex.Message);
            }
        }

        /// <summary>
        /// Checks whether the file is ready to be accessed for reading.
        /// </summary>
        /// <param name="path">The file path to check.</param>
        /// <param name="retries">Number of retries before giving up.</param>
        /// <param name="delayMs">Delay between retries in milliseconds.</param>
        /// <returns>True if the file can be opened exclusively for reading; otherwise false.</returns>
        private bool IsFileReady(string path, int retries = 5, int delayMs = 200)
        {
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                        return true;
                }
                catch
                {
                    Thread.Sleep(delayMs);
                }
            }
            return false;
        }
    }
}