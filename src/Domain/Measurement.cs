using System;

namespace ConcurrentDataFileProcessing.src.Domain
{
    /// <summary>
    /// Represents a single measurement record made by a sensor at a specific point in time.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Unique identifier of the measurement
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// DateTime when the measurement was taken.
        /// </summary>
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// Name of the sensor that recorded the measurement.
        /// </summary>
        public string Sensor { get; set; } = string.Empty;
        /// <summary>
        /// Recorded value of the measurement.
        /// </summary>
        public double Value { get; set; }
    }
}