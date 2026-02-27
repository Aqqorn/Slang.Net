param(
    [string]$SlangVersion = "2025.10.3",
    [string]$Platform = "x64",
    [switch]$Force
)

# Define paths
# Map our platform names to the ones used in download URLs
$platformArg = if ($Platform -eq "x64") { "x86_64" } elseif ($Platform -eq "ARM64") { "aarch64" } else { "x86_64" }
$downloadUrl = "https://github.com/shader-slang/slang/releases/download/v$SlangVersion/slang-$SlangVersion-windows-$platformArg.zip"
$extractDir = Join-Path $PSScriptRoot "EmbeddedLLVM"
$downloadDir = Join-Path $extractDir "downloads"
$zipFile = Join-Path $downloadDir "slang-$SlangVersion-windows-$platformArg.zip"
$extractPath = Join-Path $extractDir "slang-$SlangVersion-windows"
$platformPath = Join-Path $extractPath $Platform

# Create directories if they don't exist
if (-Not (Test-Path $downloadDir)) {
    New-Item -ItemType Directory -Path $downloadDir -Force | Out-Null
    Write-Host "Created download directory: $downloadDir"
}

# Check if files already exist and we don't need to re-download
if (-Not $Force) {
    if ((Test-Path $platformPath) -and (Test-Path "$platformPath\bin") -and (Test-Path "$platformPath\lib")) {
        Write-Host "Slang SDK already exists at $platformPath. Use -Force to redownload."
        return
    }
}

# Download the ZIP file if it doesn't exist or Force is specified
if ((-Not (Test-Path $zipFile)) -or $Force) {
    Write-Host "Downloading Slang SDK $SlangVersion for Windows ($platformArg)..."
    try {
        $ProgressPreference = 'SilentlyContinue'
	[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
        Invoke-WebRequest -Uri $downloadUrl -OutFile $zipFile
        Write-Host "Download complete: $zipFile"
    }
    catch {
        Write-Error "Failed to download Slang SDK: $_"
        exit 1
    }
}
else {
    Write-Host "Using existing download: $zipFile"
}

# Make sure the target path exists
if (-not (Test-Path $extractPath)) {
    New-Item -ItemType Directory -Path $extractPath -Force | Out-Null
}

# Make sure the platform path exists
if (-not (Test-Path $platformPath)) {
    New-Item -ItemType Directory -Path $platformPath -Force | Out-Null
}

# Extract the ZIP file to a temporary location
Write-Host "Extracting Slang SDK..."
$tempExtractPath = Join-Path $extractDir "temp_extract"
if (Test-Path $tempExtractPath) {
    Remove-Item -Recurse -Force $tempExtractPath
}
New-Item -ItemType Directory -Path $tempExtractPath -Force | Out-Null

try {
    # Extract to temp directory
    Expand-Archive -Path $zipFile -DestinationPath $tempExtractPath
    
    # Copy content to the platform-specific directory
    Write-Host "Setting up Slang SDK for $Platform..."

    # Copy each directory to the platform directory
    foreach ($dir in @("bin", "lib", "include", "cmake", "share")) {
        $sourceDir = Join-Path $tempExtractPath $dir
        $targetDir = Join-Path $platformPath $dir
        
        if (Test-Path $sourceDir) {
            if (Test-Path $targetDir) {
                Remove-Item -Recurse -Force $targetDir
            }
            Copy-Item -Path $sourceDir -Destination $platformPath -Recurse
            Write-Host "  - Copied $dir directory"
        }
    }
    
    # Copy root files
    foreach ($file in @("LICENSE", "README.md")) {
        $sourceFile = Join-Path $tempExtractPath $file
        $targetFile = Join-Path $platformPath $file
        
        if (Test-Path $sourceFile) {
            Copy-Item -Path $sourceFile -Destination $targetFile -Force
            Write-Host "  - Copied $file"
        }
    }
    
    # Clean up temp directory
    Remove-Item -Recurse -Force $tempExtractPath
    Write-Host "Extraction complete. Files copied to $platformPath"
}
catch {
    Write-Error "Failed to extract Slang SDK: $_"
    exit 1
}

# Verify extraction succeeded by checking for important files
if (-Not (Test-Path "$platformPath\bin") -or -Not (Test-Path "$platformPath\lib")) {
    Write-Error "Extraction completed but expected directories not found. Verify the archive structure."
    exit 1
}

Write-Host "Slang SDK $SlangVersion for $Platform successfully installed to $platformPath"
