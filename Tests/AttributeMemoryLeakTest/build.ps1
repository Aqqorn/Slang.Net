param(
    [Parameter(Mandatory=$true)]
    [string]$Configuration,
    
    [Parameter(Mandatory=$true)]
    [string]$Platform,

    [switch]$FromVisualStudio
)

# Slang Sdk Version
$slangVersion = "2025.16.1"

# Validate platform parameter
$validPlatforms = @("x64", "ARM64")
if ($validPlatforms -notcontains $Platform) {
    Write-Host "Error: Invalid platform '$Platform'. Valid platforms are: $($validPlatforms -join ', ')" -ForegroundColor Red
    exit 1
}

Write-Host "===== Building AttributeMemoryLeakTest =====" -ForegroundColor DarkGray

# Print parameters for debugging
Write-Host "DEBUG: -Configuration = $Configuration" -ForegroundColor Yellow
Write-Host "DEBUG: -Platform = $Platform" -ForegroundColor Yellow
Write-Host "DEBUG: -FromVisualStudio = $FromVisualStudio" -ForegroundColor Yellow

# Get solution directory
$solutionDir = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

# Directories
$testDir = $PSScriptRoot
$testOutputDir = "$solutionDir\bin\$Configuration\$Platform"
$nativeOutputDir = "$solutionDir\src\Native\bin\$Configuration\$Platform"
$nativeEmbeddedDir = "$solutionDir\src\Native\EmbeddedLLVM\slang-$slangVersion-windows\$Platform\bin"

# Create output directory if it doesn't exist
if (-not (Test-Path -Path $testOutputDir)) {
    New-Item -ItemType Directory -Path $testOutputDir -Force | Out-Null
    Write-Host "Created directory: $testOutputDir" -ForegroundColor Yellow
}

# STEP 1: Copy Native DLLs to test output directory
Write-Host "Build AttributeMemoryLeakTest(STEP 1): Copy Native DLLs..." -ForegroundColor Green

# Define native files to copy from the Native project output
$nativeOutputFiles = @(
    "$nativeOutputDir\SlangNative.dll"
)

# Add SlangNative.pdb to nativeOutputFiles if Debug configuration
if ($Configuration -eq "Debug") {
    $nativeOutputFiles += "$nativeOutputDir\SlangNative.pdb"
}

# Define embedded Slang files to copy
$embeddedSlangFiles = @(
    "$nativeEmbeddedDir\gfx.dll",
    "$nativeEmbeddedDir\slang.dll",
    "$nativeEmbeddedDir\slang-glslang.dll",
    "$nativeEmbeddedDir\slang-glsl-module.dll",
    "$nativeEmbeddedDir\slang-rt.dll"
)

# Add platform-specific files
if ($Platform -ne "ARM64") {
    # slang-llvm.dll is not available on ARM64
    $embeddedSlangFiles += "$nativeEmbeddedDir\slang-llvm.dll"
}

# Copy each native file to test output directory
foreach ($file in $nativeOutputFiles) {
    if (Test-Path $file) {
        Copy-Item $file $testOutputDir -Force
        Write-Host "Copied: $file to $testOutputDir" -ForegroundColor Cyan
    }
    else {
        Write-Host "Missing: $file" -ForegroundColor Red
        Write-Host "AttributeMemoryLeakTest build failed due to missing Native file!" -ForegroundColor Red
        exit 1
    }
}

# Copy each embedded Slang file to test output directory
foreach ($file in $embeddedSlangFiles) {
    if (Test-Path $file) {
        Copy-Item $file $testOutputDir -Force
        Write-Host "Copied: $file to $testOutputDir" -ForegroundColor Cyan
    }
    else {
        Write-Host "Missing: $file" -ForegroundColor Red
        Write-Host "AttributeMemoryLeakTest build failed due to missing embedded Slang file!" -ForegroundColor Red
        exit 1
    }
}

# STEP 2: Copy test data files
Write-Host "Build AttributeMemoryLeakTest(STEP 2): Copy test data files..." -ForegroundColor Green

$testDataFiles = @(
    "$testDir\ComprehensiveSlangTest.slang"
)

foreach ($file in $testDataFiles) {
    if (Test-Path $file) {
        Copy-Item $file $testOutputDir -Force
        Write-Host "Copied: $file to $testOutputDir" -ForegroundColor Cyan
    }
    else {
        Write-Host "Warning: Test data file not found: $file" -ForegroundColor Yellow
    }
}

# Verify the output files
Write-Host "Verifying output files..." -ForegroundColor Green

# List of files we expect to find in the output directory
$expectedFiles = @(
    "$testOutputDir\AttributeMemoryLeakTest.exe",
    "$testOutputDir\SlangNative.dll",
    "$testOutputDir\slang.dll",
    "$testOutputDir\gfx.dll"
)

foreach ($file in $expectedFiles) {
    if (Test-Path $file) {
        Write-Host "Verified: $file" -ForegroundColor Green
    }
    else {
        Write-Host "Missing expected file: $file" -ForegroundColor Yellow
    }
}

Write-Host "AttributeMemoryLeakTest build completed successfully!" -ForegroundColor Green