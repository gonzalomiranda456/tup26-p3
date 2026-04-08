using System;
using System.Collections.Generic;
using System.IO;

// Flujo de funciones

try
{
    AppConfig config = ParseArgs(args);

    string inputData = ReadInput(config.InputFile);

    CsvData parsedData = ParseDelimited(inputData, config.Delimiter, config.NoHeader);

    WriteOutput(config.OutputFile, inputData);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}

//lectura de argumentos y genera AppConfig
AppConfig ParseArgs(string[] arguments)
{
    string? input = null;
    string? output = null;
    string delim = ",";
    bool noHeader = false;
    List<SortField> sortFields = new List<SortField>();

    for (int i = 0; i < arguments.Length; i++)
    {
        string arg = arguments[i];

        if (arg == "-h" || arg == "--help")
        {
            Console.WriteLine("Uso: sortx [input] [output] [-b campo:tipo:orden] [-d delimitador] [-nh]");
            Environment.Exit(0);
        }
        else if (arg == "-d" || arg == "--delimiter")
        {
            delim = arguments[++i];
        }
        else if (arg == "-nh" || arg == "--no-header")
        {
            noHeader = true;
        }
        else if (arg == "-b" || arg == "--by")
        {
            string rawBy = arguments[++i];
            string[] parts = rawBy.Split(':');

            string name = parts[0];
            bool isNumeric = parts.Length > 1 && parts[1] == "num";
            bool isDesc = parts.Length > 2 && parts[2] == "desc";

            sortFields.Add(new SortField(name, isNumeric, isDesc));
        }
        else if (arg == "-i" || arg == "--input")
        {
            input = arguments[++i];
        }
        else if (arg == "-o" || arg == "--output")
        {
            output = arguments[++i];
        }
        else if (!arg.StartsWith("-"))
        {
            if (input == null) input = arg;
            else if (output == null) output = arg;
        }
    }

    return new AppConfig(input, output, delim, noHeader, sortFields);
}
// lectura del archivo 
string ReadInput(string? filePath)
{
    if (string.IsNullOrEmpty(filePath))
    {
        return Console.In.ReadToEnd(); 
    }
    return File.ReadAllText(filePath); 
}

// Creacion del archivo de datos
void WriteOutput(string? filePath, string content)
{
    if (string.IsNullOrEmpty(filePath))
    {
        Console.Write(content); 
    }
    else
    {
        File.WriteAllText(filePath, content); 
    }
}

// Transforma el texto crudo en una tabla estructurada (cabeceras y filas)
CsvData ParseDelimited(string rawText, string delimiter, bool noHeader)
{
    string[] lines = rawText.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);

    List<string[]> rows = new List<string[]>();
    string[] headers = Array.Empty<string>();

    if (lines.Length == 0) return new CsvData(headers, rows);

    int startIndex = 0;

    if (!noHeader)
    {
        headers = lines[0].Split(delimiter);
        startIndex = 1; 
    }
    else
    {
        int colCount = lines[0].Split(delimiter).Length;
        headers = new string[colCount];
        for (int i = 0; i < colCount; i++) headers[i] = $"Col{i}";
    }
    for (int i = startIndex; i < lines.Length; i++)
    {
        rows.Add(lines[i].Split(delimiter));
    }

    return new CsvData(headers, rows);
}

// almanecer criterios de ordenamientos 
record SortField(string Name, bool Numeric, bool Descending);
record AppConfig(
    string? InputFile,
    string? OutputFile,
    string Delimiter,
    bool NoHeader,
    List<SortField> SortFields
);
record CsvData(string[] Headers, List<string[]> Rows);