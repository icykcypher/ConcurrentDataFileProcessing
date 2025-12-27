using System.Globalization;

namespace MiniOrderApp.Import.Parsers;

public class CsvParser<T>(Func<string[], T> map) : ICsvParser<T>
{
        public IEnumerable<T> Parse(Stream stream)
        {
                using var reader = new StreamReader(stream);
                string? line;
                bool headerSkipped = false;

                while ((line = reader.ReadLine()) != null)
                {
                        if (!headerSkipped)
                        {
                                headerSkipped = true;
                                continue;
                        }

                        var parts = line.Split(',');
                        yield return map(parts);
                }
        }
}