using System.CommandLine;

namespace CS2TS;

public static class CliOptions
{
    public static string[]? Assemblies { get; private set; }
    public static string? OutputPath { get; private set; }

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
            getDefaultValue: () => "typescript/my-ts-library"
        );

        var rootCommand = new RootCommand("CS2TS - C# to TypeScript model generator");
        rootCommand.AddOption(assemblyOption);
        rootCommand.AddOption(outputOption);

        // Parse and apply defaults
        var result = rootCommand.Parse(args);
        
        Assemblies = result.GetValueForOption(assemblyOption) ?? [];
        OutputPath = result.GetValueForOption(outputOption);
    }
}
