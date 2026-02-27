# Building From Source

## Prerequisites
- Visual Studio 2022 with C++ toolchain
- .NET 9.0 SDK
- PowerShell

## Canonical Build Flow

### 1) Building SlangNative 
	-	This step can be skipped as Building Slang.Sdk automates this step
From the repo root:

```powershell
cd src
.\all-platforms.ps1 -script Native\build.ps1
```

This builds native outputs for supported Windows platforms.

### 2) Building Slang.Sdk .NET project
Build the .NET project normally (solution or project build). 
- This will also build SlangNative if needed

Native changes are automatically included by the .NET project packaging/output pipeline.

Examples:

```powershell
# from repo root
dotnet build .\src\Slang.Sdk\Slang.Sdk.csproj -c Debug

dotnet build .\src\Slang.Sdk\Slang.Sdk.csproj -c Release
```

## Notes
- `Tests/AttributeMemoryLeakTest` is deprecated.
- `Samples-Old` is deprecated/legacy content.
