# CS to TS

## What is it?

A tool for converting C# models to TypeScript types.

## Runtime

- Targets `.NET 10`
- Verified on `king95-linux`
- No separate .NET 9 runtime install is required

## Usage

```bash
dotnet restore
dotnet build CS2TS.sln
dotnet run --project CS2TS.csproj -- --help
```

Generate output to a custom folder:

```bash
dotnet run --project CS2TS.csproj -- --assembly ./path/to/MyModels.dll --out ./typescript/out
```

Open the generated output folder after generation:

```bash
dotnet run --project CS2TS.csproj -- --assembly ./path/to/MyModels.dll --out ./typescript/out --open
```

`--open` is off by default. When enabled it uses the current OS opener:

- Windows: `explorer.exe`
- macOS: `open`
- Linux: `xdg-open`

## How It Works

Configure one or more assemblies that contain your interfaces and DTOs, then run the generator. After generation, run `tsc`, package the result, and deploy the library.

## Why

Automation.

## Contact Us

lancer1977@gmail.com
