using System;

namespace ConcurrentDataFileProcessing.src.Domain
{
    /// <summary>
    /// Represents a single measurement record made by a sensor at a specific point in time.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public long Number { get; set; }

        /// <summary>
        /// Timestamp of the measurement (from epoch milliseconds)
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Surface level (0.0 in your example)
        /// </summary>
        public double Surface { get; set; }

        /// <summary>
        /// Step (forecast step, usually 0)
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Temperature at 2 meters (Kelvin)
        /// </summary>
        public double Temperature2m { get; set; }
    }
}