# Concurrent File Processor (.NET Framework 4.8)

## Description

The **Concurrent File Processor** is a console application for parallel processing of measurement files (CSV/JSON).  
It implements a **Producer-Consumer pattern** and demonstrates safe multithreading with resource synchronization.

The program performs the following tasks:

1. Monitors the `input` folder and automatically enqueues new files for processing.
2. Multiple workers (default: 4) process files concurrently:
   - Parse data (`FileParser`)
   - Save measurements to a SQLite database (`SqliteRepository`)
   - Update a temperature aggregator (`TemperatureAggregator`)
   - Move files to `processed` or `error` folders

---

## Parallelism and Synchronization

- **Job queue** (`BlockingCollection<FileProcessingJob>`) ensures safe communication between producer (folder watcher) and consumers (worker threads).
- **Database access** is synchronized using a `lock` to prevent concurrent writes from multiple workers.
- **Shared aggregator** (`TemperatureAggregator`) is thread-safe, accumulating measurements from all workers concurrently.
- **File processing coordination**:
  - Prevents multiple workers from processing the same file
  - Handles partially written files by checking readiness before processing

This approach avoids race conditions, deadlocks, and data corruption, making the program robust for real-world concurrent workloads.

---

## Usage

1. Place CSV or JSON files with measurements in the `input` folder.
2. Run the executable. The console displays queueing and processing messages.
3. Processed files are moved to `output/processed`; files with errors go to `output/error`.
4. After processing, the program calculates and prints average temperatures per month and overall.
