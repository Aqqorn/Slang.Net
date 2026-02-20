param(
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",

    [Parameter()]
    [ValidateSet("Smoke", "Integration", "All")]
    [string]$Suite = "All",

    [switch]$BuildNative,
    [switch]$Help
)

if ($Help) {
    Write-Host @"
Slang.Sdk Test Runner

Usage:
  .\Tests\run_tests.ps1 [-Configuration Debug|Release] [-Suite Smoke|Integration|All] [-BuildNative]

Examples:
  .\Tests\run_tests.ps1
  .\Tests\run_tests.ps1 -Suite Smoke
  .\Tests\run_tests.ps1 -Suite Integration -BuildNative
"@
    exit 0
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $repoRoot "Tests\Slang.Sdk.Tests\Slang.Sdk.Tests.csproj"

if ($BuildNative) {
    Write-Host "Building native dependencies..." -ForegroundColor Cyan
    Push-Location (Join-Path $repoRoot "src")
    try {
        .\all-platforms.ps1 -Script Native\build.ps1
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
    finally {
        Pop-Location
    }
}

$filter = switch ($Suite) {
    "Smoke" { "Category=Smoke" }
    "Integration" { "Category=Integration" }
    default { "Category!=Stress" }
}

Write-Host "Running $Suite tests ($Configuration)..." -ForegroundColor Cyan

dotnet test $testProject -c $Configuration --filter $filter
exit $LASTEXITCODE
