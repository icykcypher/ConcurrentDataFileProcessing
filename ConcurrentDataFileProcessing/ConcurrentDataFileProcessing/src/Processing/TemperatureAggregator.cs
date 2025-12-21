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

        public double? GetMonthlyAverage(int year, int month)
        {
            lock (_lock)
            {
                var monthly = _allMeasurements
                    .Where(m => m.Timestamp.Year == year && m.Timestamp.Month == month)
                    .ToList();

                if (monthly.Count == 0)
                    return null;

                return monthly.Average(m => m.Temperature2m);
            }
        }

        public double? GetYearlyAverage(int year)
        {
            lock (_lock)
            {
                var yearly = _allMeasurements
                    .Where(m => m.Timestamp.Year == year)
                    .ToList();

                if (yearly.Count == 0)
                    return null;

                return yearly.Average(m => m.Temperature2m);
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
