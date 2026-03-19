using System.Diagnostics;
using CS2TS;

var cliArgs = args.Length > 0 ? args : ["--help"];

if (cliArgs.Contains("--help") || cliArgs.Contains("-h"))
{
    CliOptions.PrintHelp();
    return 0;
}

CliOptions.Parse(cliArgs);

var path = CliOptions.OutputPath ?? Path.Combine("typescript", "my-ts-library");
var fullOutputPath = Path.GetFullPath(path);

TypeScriptInterfacesExtension.GenerateTypeScriptInterfaces(path);
Console.WriteLine($"Generated TypeScript output at: {fullOutputPath}");

if (CliOptions.OpenOutput)
{
    OpenOutputDirectory(fullOutputPath);
}

return 0;

static void OpenOutputDirectory(string outputPath)
{
    ProcessStartInfo startInfo;

    if (OperatingSystem.IsWindows())
    {
        startInfo = new ProcessStartInfo("explorer.exe", outputPath);
    }
    else if (OperatingSystem.IsMacOS())
    {
        startInfo = new ProcessStartInfo("open", outputPath);
    }
    else if (OperatingSystem.IsLinux())
    {
        startInfo = new ProcessStartInfo("xdg-open", outputPath);
    }
    else
    {
        Console.WriteLine($"Output generated at: {outputPath}");
        Console.WriteLine("Automatic folder opening is not supported on this OS.");
        return;
    }

    startInfo.UseShellExecute = false;

    try
    {
        using var process = Process.Start(startInfo);
        if (process == null)
        {
            Console.WriteLine($"Output generated at: {outputPath}");
            Console.WriteLine("Unable to open output directory automatically.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Output generated at: {outputPath}");
        Console.WriteLine($"Unable to open output directory automatically: {ex.Message}");
    }
}
