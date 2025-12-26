namespace MiniOrderApp.Import.Parsers;

public interface ICsvParser<T>
{
        IEnumerable<T> Parse(Stream stream);
}