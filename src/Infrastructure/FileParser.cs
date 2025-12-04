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
            if (header.Contains("valid_time") || header.Contains("t2m") || header.Contains("surface") || header.Contains("step"))
            {
                start = 1;
            }

            for (int i = start; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length < 4)
                    continue;

                if (!long.TryParse(parts[0], out long epochMillis))
                    continue;

                DateTime ts = DateTimeOffset.FromUnixTimeMilliseconds(epochMillis).UtcDateTime;

                if (!double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double t2m))
                    continue;

                if (!double.TryParse(parts[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double surface))
                    continue;

                if (!int.TryParse(parts[3], out int step))
                    continue;

                result.Add(new Measurement
                {
                    Number = i,
                    Timestamp = ts,
                    Temperature2m = t2m - 273.15,
                    Surface = surface,
                    Step = step
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
                int i = 1;

                foreach (var el in arr)
                {
                    if (el["valid_time"] == null || el["t2m"] == null)
                        continue;

                    if (!long.TryParse(el["valid_time"].ToString(), out long unixMs))
                        continue;

                    var ts = DateTimeOffset.FromUnixTimeMilliseconds(unixMs).UtcDateTime;

                    if (!double.TryParse(el["t2m"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out double t2mK))
                        continue;

                    double t2mC = t2mK - 273.15;

                    list.Add(new Measurement
                    {
                        Number = i,
                        Timestamp = ts,
                        Surface = el["surface"] != null ? (double)el["surface"] : 0,
                        Step = el["step"] != null ? (int)el["step"] : 0,
                        Temperature2m = t2mC
                    });

                    i++;
                }

                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] ParseJson exception: " + ex.Message);
                return null;
            }
        }
    }
}