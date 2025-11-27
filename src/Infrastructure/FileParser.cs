using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Collections.Generic;
using ConcurrentDataFileProcessing.src.Domain;

namespace ConcurrentDataFileProcessing.src.Infrastructure
{
    /// <summary>
    /// Class that provides methods to parse measurement data from CSV or JSON files.
    /// </summary>
    public static class FileParser
    {
        /// <summary>
        /// Parses a file and returns a list of Measurement objects.
        /// Supports CSV and JSON file formats.
        /// </summary>
        /// <param name="path">The full path to the file to parse.</param>
        /// <returns>
        /// A list of Measurement objects if parsing was successful; otherwise, <c>null</c>.
        /// </returns>
        public static List<Measurement> Parse(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            switch (ext)
            {
                case ".csv":
                    return ParseCsv(path);

                case ".json":
                    Console.WriteLine("Parsing json");
                    return ParseJson(path);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Parses a CSV file containing measurement data.
        /// Expected columns: Timestamp, Sensor, Value.
        /// </summary>
        /// <param name="path">The CSV file path.</param>
        /// <returns>
        /// A list of Measurement objects or <c>null</c> if file is empty or invalid.
        /// </returns>
        private static List<Measurement> ParseCsv(string path)
        {
            var lines = File.ReadAllLines(path)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToArray();

            if (lines.Length == 0)
                return null;

            var result = new List<Measurement>();
            int start = 0;

            var header = lines[0].ToLowerInvariant();
            if (header.Contains("timestamp") || header.Contains("sensor"))
            {
                start = 1;
            }

            for (int i = start; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length < 3)
                    continue;

                DateTime ts;
                if (!DateTime.TryParse(parts[0], null, DateTimeStyles.AdjustToUniversal, out ts))
                {
                    long epoch;
                    if (long.TryParse(parts[0], out epoch))
                    {
                        ts = new DateTime(1970, 1, 1).AddSeconds(epoch);
                    }
                    else
                    {
                        continue;
                    }
                }

                var sensor = parts[1].Trim();

                double val;
                if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                    continue;

                result.Add(new Measurement
                {
                    Id = i,
                    Timestamp = ts,
                    Sensor = sensor,
                    Value = val
                });
            }

            return result;
        }

        /// <summary>
        /// Parses a JSON file containing an array of measurement objects.
        /// Each JSON object should contain "Timestamp", "Sensor", and "Value" properties.
        /// </summary>
        /// <param name="path">The JSON file path.</param>
        /// <returns>
        /// A list of Measurement objects or null if file is empty or parsing fails.
        /// </returns>
        private static List<Measurement> ParseJson(string path)
        {
            try
            {
                var txt = File.ReadAllText(path);
                var arr = JArray.Parse(txt);

                var list = new List<Measurement>();
                var i = 1;
                foreach (var el in arr)
                {
                    if (el["Timestamp"] == null || el["Sensor"] == null || el["Value"] == null)
                        continue;

                    DateTime ts;
                    if (!DateTime.TryParse((string)el["Timestamp"], null, DateTimeStyles.AdjustToUniversal, out ts))
                        continue;

                    var sensor = (string)el["Sensor"] ?? "";

                    double value = 0;
                    double.TryParse(el["Value"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out value);

                    list.Add(new Measurement
                    {
                        Id = i,
                        Timestamp = ts,
                        Sensor = sensor,
                        Value = value
                    });

                    i++;
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}