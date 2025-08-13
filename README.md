# CSVToJSONConverter

A tiny, streaming CSV → JSON library for .NET that converts each CSV row to a JSON object and writes the result as a JSON array.  
It reads the CSV line-by-line (async) and writes directly to an `Utf8JsonWriter` to keep memory usage modest for large files.

---

## How it works (at a glance)

**CSV parsing**  
- First non-blank line becomes **headers**.  
- Subsequent lines are split by a compiled, source-generated regex that respects quoted fields (commas inside quotes don’t split).

**JSON writing**  
- Emits a JSON array of objects, one per row, mapping `header -> value`.  
- All values are written as **strings**.

---

## Requirements

- .NET 9 or later (uses `[GeneratedRegex]`, regex span-based enumeration, span splits).  
---

## Installation

Add the two source files to your project:

- `CSVParser.cs` — streaming CSV reader
- `JSONConverter.cs` — CSV → JSON writer

---

## Usage

```csharp
using CSVToJSONConverter;

var converter = new JSONConverter();
await converter.ConvertToJsonAsync(
    csvFilePath: "input.csv",
    outputFilePath: "output.json");
```
This will produce a JSON array like:

```json
[
  { "Header1": "value", "Header2": "value2" },
  { "Header1": "value", "Header2": "value2" }
]
```

## Performance
On my machine (.NET 9, Release), processed ~139,691 rows in ~125 ms (~1.12 million rows/sec) with warm OS cache and null output to.
The same benchmark without warm OS cache and IO operations as usual is ~140ms (~1.0 million rows/sec).
<img width="1963" height="583" alt="Benchmark" src="https://github.com/user-attachments/assets/358595a0-3591-4b54-bda5-7e674f8f7032" />
