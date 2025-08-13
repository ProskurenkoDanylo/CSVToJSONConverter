using CommandLine;
using System.Text.Json;
namespace CSVToJSONConverter;

public class JSONConverter
{
    public JSONConverter() { }

    public async Task ConvertToJsonAsync(string csvFilePath, string outputFilePath)
    {
        var parser = new CSVParser();
        await using var fs = File.Create(outputFilePath);
        using var json = new Utf8JsonWriter(fs);
        json.WriteStartArray();
        await foreach (var row in parser.StreamRowsAsync(csvFilePath))
        {
            json.WriteStartObject();

            int count = Math.Min(parser.Headers.Count, row.Length);
            for (int i = 0; i < count; i++)
            {
                json.WriteString(parser.Headers[i], row[i]);
            }
            json.WriteEndObject();
        }

        json.WriteEndArray();
        await json.FlushAsync();
    }
}
