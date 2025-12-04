using System;
using System.Linq;
using System.Collections.Generic;
using ConcurrentDataFileProcessing.src.Domain;

namespace ConcurrentDataFileProcessing.src.Processing
{
    public class TemperatureAggregator
    {
        private readonly object _lock = new object();
        private readonly List<Measurement> _allMeasurements = new List<Measurement>();

        public void AddMeasurements(IEnumerable<Measurement> measurements)
        {
            lock (_lock)
            {
                _allMeasurements.AddRange(measurements);
            }
        }

        public void PrintMonthlyAndYearlyAverage()
        {
            lock (_lock)
            {
                var monthly = _allMeasurements
                    .GroupBy(m => m.Timestamp.Month)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        Month = g.Key,
                        Avg = g.Average(m => m.Temperature2m)
                    });

                Console.WriteLine("Average temperature by monthes: ");
                foreach (var m in monthly)
                    Console.WriteLine($"Month {m.Month}: {m.Avg:F2} °C");

                if (_allMeasurements.Count > 0)
                {
                    var yearly = _allMeasurements.Average(m => m.Temperature2m);
                    Console.WriteLine($"Average year temperature: {yearly:F2} °C");
                }
            }
        }
    }
}