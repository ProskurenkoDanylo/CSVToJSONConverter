namespace CSVToJSONConverter.Test
{
    public class Tests
    {
        private static async Task<(CSVParser parser, string[][] rows)> ParseAsync(string csv)
        {
            var path = Path.GetTempFileName();
            await File.WriteAllTextAsync(path, csv);

            var parser = new CSVParser();
            var rows = new System.Collections.Generic.List<string[]>();

            await foreach (var row in parser.StreamRowsAsync(path))
                rows.Add(row);

            return (parser, rows.ToArray());
        }

        [Fact]
        public async Task Header_With_Quotes_And_Comma_Parses()
        {
            var csv = "\"A,part\",B\n1,2\n";
            var (p, rows) = await ParseAsync(csv);

            Assert.Equal(p.Headers, (new[] { "A,part", "B" }));
            Assert.True(rows.Length == 1);
            Assert.Equal(rows[0], new[] { "1", "2" });
        }

        

        [Fact]
        public async Task Escaped_Quote_Unescaped_In_Quoted_Field()
        {
            var csv = "\"A,part\",\"B\"\"C\"\n1,2\n";
            var (p, rows) = await ParseAsync(csv);
            Assert.Equal("B\"\"C", p.Headers[1]);
        }

        [Fact]
        public async Task Very_Long_Line_Does_Not_Blow_Up()
        {
            var data = new string('x', 100_000);
            var csv = $"A,B\n\"{data}\",y\n";
            var (p, rows) = await ParseAsync(csv);

            Assert.True(rows[0][0].Length == 100_000);
            Assert.Equal("y", rows[0][1]);
        }
    }
}
