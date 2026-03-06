using System.CommandLine;
using System.Diagnostics;
using CS2TS;

var cliArgs = args.Length > 0 ? args : ["--help"];
CliOptions.Parse(cliArgs);

var path = CliOptions.OutputPath ?? "typescript/my-ts-library";
Console.WriteLine("Hello, World!");
TypeScriptInterfacesExtension.GenerateTypeScriptInterfaces(path);

if (OperatingSystem.IsWindows())
{
    Process.Start("explorer.exe", Directory.GetCurrentDirectory() + path);
}
