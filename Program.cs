 

using System.Diagnostics;
using CS2TS;

var path = "typescript/my-ts-library";
Console.WriteLine("Hello, World!");
TypeScriptInterfacesExtension.GenerateTypeScriptInterfaces(path);
Process.Start("explorer.exe",Directory.GetCurrentDirectory()+ path);


