# Slang.Sdk.Tests (xUnit)

## Categories
- `Smoke` - fast environment/runtime checks
- `Integration` - compile/session behavior tests
- `Stress` - long-running or unstable stress tests

## Commands

Run all non-skipped tests:
```powershell
dotnet test .\Tests\Slang.Sdk.Tests\Slang.Sdk.Tests.csproj -c Debug
```

Run smoke only:
```powershell
dotnet test .\Tests\Slang.Sdk.Tests\Slang.Sdk.Tests.csproj -c Debug --filter "Category=Smoke"
```

Run integration only:
```powershell
dotnet test .\Tests\Slang.Sdk.Tests\Slang.Sdk.Tests.csproj -c Debug --filter "Category=Integration"
```

Run stress (when enabled):
```powershell
dotnet test .\Tests\Slang.Sdk.Tests\Slang.Sdk.Tests.csproj -c Debug --filter "Category=Stress"
```
