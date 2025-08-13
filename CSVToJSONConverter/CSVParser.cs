using System.Text.RegularExpressions;

namespace CSVToJSONConverter;

public partial class CSVParser
{
    public List<string> Headers { get; private set; } = new();

    [GeneratedRegex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")]
    private static partial Regex CsvSplitRegex();

    public async IAsyncEnumerable<string[]> StreamRowsAsync(string filePath)
    {
        Headers.Clear();
        using StreamReader reader = new StreamReader(filePath);
        string? line;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            var span = line.AsSpan();

            if (Headers.Count == 0)
            {
                foreach (var seg in CsvSplitRegex().EnumerateSplits(span))
                {
                    Headers.Add(span[seg].Trim('"').ToString());
                }
                continue;
            }

            var values = new List<string>(Headers.Count);
            foreach (var seg in CsvSplitRegex().EnumerateSplits(span))
                values.Add(span[seg].Trim('"').ToString());
            yield return values.ToArray();
        }
    }
}
