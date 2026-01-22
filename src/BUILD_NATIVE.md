# Building Native Dependencies

This document describes how to build the native C++ dependencies for Slang.Net.

## Prerequisites

- Visual Studio 2022 with C++ workload
- PowerShell 5.1 or later

## Build Instructions

1. Open a PowerShell terminal
2. Navigate to the `src` directory:
   ```powershell
   cd path\to\Slang.Net\src
   ```
3. Run the build script:
   ```powershell
   .\all-platforms.ps1 -script Native/build.ps1
   ```

This will build the `SlangNative` library for all supported platforms.

## Output

The compiled native binaries will be placed in the appropriate runtime directories for the Slang.Sdk NuGet package.
