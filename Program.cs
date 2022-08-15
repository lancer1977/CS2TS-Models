// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using InterfacesToTS;

var path = ".\\src";
Console.WriteLine("Hello, World!");
TypeScriptInterfacesExtension.GenerateTypeScriptInterfaces(path);
Process.Start("explorer.exe",Directory.GetCurrentDirectory()+ path);


