param(
    [Parameter(Mandatory=$true)]
    [string]$Configuration,
    
    [Parameter(Mandatory=$true)]
    [string]$Platform,

    [switch]$FromVisualStudio
)

# Slang Sdk Version
$slangVersion = "2025.13.2"

# Validate platform parameter
$validPlatforms = @("x64", "ARM64")
if ($validPlatforms -notcontains $Platform) {
    Write-Host "Error: Invalid platform '$Platform'. Valid platforms are: $($validPlatforms -join ', ')" -ForegroundColor Red
    exit 1
}

Write-Host "===== Building SlangNative Library =====" -ForegroundColor DarkGray

# Directories
$nativeDir = $PSScriptRoot

# STEP 1: Download Slang SDK if not already present
Write-Host "Build SlangNative(STEP 1): Downloaded Slang SDK..." -ForegroundColor DarkBlue
& "$nativeDir\download-slang-sdk.ps1" -SlangVersion $slangVersion -Platform $Platform
 
#Copy the Slang SDK to the output directory
$sdkPath = Join-Path $nativeDir "EmbeddedLLVM\slang-$slangVersion-windows\$Platform\bin\*.dll"
$slangSdkOutputDir = Join-Path $nativeDir "bin\$Configuration\$Platform\"

if (-not (Test-Path $slangSdkOutputDir)) {
    New-Item -ItemType Directory -Path $slangSdkOutputDir -Force | Out-Null
    Write-Host "Created directory: $slangSdkOutputDir" -ForegroundColor Yellow
}

# Copy all files from the Slang SDK bin directory
$sdkBinPath = Join-Path $nativeDir "EmbeddedLLVM\slang-$slangVersion-windows\$Platform\bin\*"
foreach ($file in (Get-ChildItem $sdkBinPath -File)) {
    Write-Host "Copying Slang SDK file: $($file.Name)" -ForegroundColor Green
    Copy-Item -Path $file.FullName -Destination $slangSdkOutputDir

    if (-not $?) {
        Write-Host "Failed to copy Slang SDK file: $($file.Name)" -ForegroundColor Red
        exit 1
    }
}


# STEP 2: Build SlangNative project for the specified platform
Write-Host "Build SlangNative(STEP 2): MSBuild SlangNative project $Configuration|$Platform..." -ForegroundColor DarkBlue

# MSBuild paths
$msbuildPaths = @(
    "${env:ProgramFiles}\Microsoft Visual Studio\18\Insiders\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Preview\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
    "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe"
)
$msbuildPath = $null
foreach ($path in $msbuildPaths) {
    if (Test-Path $path) {
        $msbuildPath = $path
        break
    }
}

if (-not $msbuildPath) {
    Write-Host "Could not find MSBuild.exe in any of the expected locations." -ForegroundColor Red
    Write-Host "Make sure Visual Studio is installed with C++ development tools." -ForegroundColor Red
    exit 1
}

& $msbuildPath "$nativeDir\SlangNative.vcxproj" /p:Configuration=$Configuration /p:Platform=$Platform /t:Build /p:PreBuildDisabled=true /p:PostBuildDisabled=true

if (-not $?) {
    Write-Host "SlangNative build failed!" -ForegroundColor Red
    exit 1
}

# Ensure SlangNative.lib exists
$nativeOutputDir = Join-Path $nativeDir "bin\$Configuration\$Platform"
if (-not (Test-Path $nativeOutputDir)) {
    Write-Host "Build SlangNative failed due to missing output directory after msbuild call: $nativeOutputDir" -ForegroundColor Red
    exit 1
}

# Check alternative locations
$nativeOutputFiles = @(
    "$nativeOutputDir\gfx.dll",
    "$nativeOutputDir\slang.dll",
    "$nativeOutputDir\slang-glslang.dll",
    "$nativeOutputDir\slang-glsl-module.dll",
    "$nativeOutputDir\SlangNative.dll",
    "$nativeOutputDir\SlangNative.lib",
    "$nativeOutputDir\slang-rt.dll",
    "$nativeOutputDir\slangc.exe"
)

if ($Platform -eq "x64") {
    $nativeOutputFiles += "$nativeOutputDir\slang-llvm.dll"
}
    
foreach ($file in $nativeOutputFiles) {
    if (Test-Path $file) {
        Write-Host "Verified: $file" -ForegroundColor Cyan
    }
    else {
        Write-Host "Missing: $file" -ForegroundColor Red
        Write-Host "SlangNative build failed due to missing output file!" -ForegroundColor Red
        exit 1
    }
}