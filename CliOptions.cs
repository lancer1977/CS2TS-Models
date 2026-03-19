using System.CommandLine;

namespace CS2TS;

public static class CliOptions
{
    public static string[]? Assemblies { get; private set; }
    public static string? OutputPath { get; private set; }
    public static bool OpenOutput { get; private set; }

    public static void Parse(string[] args)
    {
        var assemblyOption = new Option<string[]>(
            name: "--assembly",
            description: "Path to assembly to scan (can be specified multiple times)",
            getDefaultValue: () => []
        )
        {
            AllowMultipleArgumentsPerToken = true
        };

        var outputOption = new Option<string>(
            name: "--out",
            description: "Output path for TypeScript files",
            getDefaultValue: () => Path.Combine("typescript", "my-ts-library")
        );

        var openOption = new Option<bool>(
            name: "--open",
            description: "Open the output folder after generation"
        );

        var rootCommand = new RootCommand("CS2TS - C# to TypeScript model generator");
        rootCommand.AddOption(assemblyOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(openOption);

        var result = rootCommand.Parse(args);

        Assemblies = result.GetValueForOption(assemblyOption) ?? [];
        OutputPath = result.GetValueForOption(outputOption);
        OpenOutput = result.GetValueForOption(openOption);
    }

    public static void PrintHelp()
    {
        Console.WriteLine("CS2TS - C# to TypeScript model generator");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --assembly <path>   Path to assembly to scan (repeatable)");
        Console.WriteLine("  --out <path>        Output path for TypeScript files");
        Console.WriteLine("  --open              Open the output folder after generation");
        Console.WriteLine("  --help              Show help information");
    }
}
