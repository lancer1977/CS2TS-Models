using System.Reflection;
using System.Text.Json;

namespace CS2TS;

public static class Constants
{
    private static Assembly[]? _assemblies;
    private static readonly object _lock = new();

    public static Assembly[] Assemblies
    {
        get
        {
            if (_assemblies == null)
            {
                lock (_lock)
                {
                    if (_assemblies == null)
                    {
                        _assemblies = LoadAssemblies();
                    }
                }
            }
            return _assemblies;
        }
    }

    private static Assembly[] LoadAssemblies()
    {
        // 1. Check CLI args first (highest priority)
        var cliAssemblies = CliOptions.Assemblies ?? [];
        if (cliAssemblies.Length > 0)
        {
            return LoadFromPaths(cliAssemblies);
        }

        // 2. Check appsettings.json
        var configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(configPath))
        {
            try
            {
                var json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
                if (config != null && config.TryGetValue("assemblies", out var assembliesElement))
                {
                    var paths = JsonSerializer.Deserialize<string[]>(assembliesElement.GetRawText());
                    if (paths != null && paths.Length > 0)
                    {
                        return LoadFromPaths(paths);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load appsettings.json: {ex.Message}");
            }
        }

        // 3. Fallback - no assemblies configured
        Console.WriteLine("Error: No assemblies configured and fallback not available.");
        Console.WriteLine("Usage: dotnet run -- --assembly ./path/to/MyModels.dll --out ./typescript/output");
        Environment.Exit(1);
        return [];
    }

    private static Assembly[] LoadFromPaths(string[] paths)
    {
        var assemblies = new List<Assembly>();
        var errors = new List<string>();

        foreach (var path in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                errors.Add("Assembly path is empty");
                continue;
            }

            if (!File.Exists(path))
            {
                errors.Add($"Assembly not found: '{path}'. Use absolute or relative path from: {Directory.GetCurrentDirectory()}");
                continue;
            }

            try
            {
                var assembly = Assembly.LoadFrom(Path.GetFullPath(path));
                assemblies.Add(assembly);
                Console.WriteLine($"Loaded assembly: {assembly.GetName().Name}");
            }
            catch (Exception ex)
            {
                errors.Add($"Failed to load assembly '{path}': {ex.Message}");
            }
        }

        if (errors.Count > 0)
        {
            Console.WriteLine("Assembly loading errors:");
            foreach (var error in errors)
            {
                Console.WriteLine($"  - {error}");
            }
            Console.WriteLine("Usage: dotnet run -- --assembly ./path/to/MyModels.dll --out ./typescript/output");
            Environment.Exit(1);
        }

        return assemblies.ToArray();
    }

    public static readonly Type[] NonPrimitivesExcludeList = new Type[4]
    {
        typeof(object),
        typeof(string),
        typeof(decimal),
        typeof(void)
    };

    public static readonly IDictionary<Type, string> ConvertedTypes = new Dictionary<Type, string>
    {
        [typeof(Guid)] = "string",
        [typeof(string)] = "string",
        [typeof(char)] = "string",
        [typeof(byte)] = "number",
        [typeof(sbyte)] = "number",
        [typeof(short)] = "number",
        [typeof(ushort)] = "number",
        [typeof(int)] = "number",
        [typeof(uint)] = "number",
        [typeof(long)] = "number",
        [typeof(ulong)] = "number",
        [typeof(float)] = "number",
        [typeof(double)] = "number",
        [typeof(decimal)] = "number",
        [typeof(bool)] = "boolean",
        [typeof(object)] = "any",
        [typeof(void)] = "void",
        [typeof(DateTime)] = "Date",
        [typeof(Uri)] = "string",
        [typeof(Type)] = "any"
    };
}
