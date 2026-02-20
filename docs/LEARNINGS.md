# Slang.Sdk Learnings Log

> Purpose: Capture practical findings (especially from `slangc` experiments and API behavior) so we don’t rediscover the same issues.

## Entry Template

### YYYY-MM-DD — Short title
**Context**
- What was being tested and why.

**Command(s)**
```powershell
# exact commands used
```

**Observed Behavior**
- Actual output/results.

**Expected Behavior**
- What we expected.

**Conclusion**
- What this means for Slang.Sdk.

**Action Items**
- [ ] Add/adjust test(s)
- [ ] Fix code (if needed)
- [ ] Update docs/samples

---

### 2026-02-13 — Core target matrix stays reliable for compute smoke
**Context**
- Validate that smoke-level compile checks cover key backend targets with one minimal compute shader.

**Command(s)**
```powershell
dotnet test Tests/Slang.Sdk.Tests/Slang.Sdk.Tests.csproj --filter "Slangc_ShouldCompile_ComputeShader_OnCoreTargets"
```

**Observed Behavior**
- `hlsl/cs_5_0`, `glsl/glsl_450`, and `spirv/glsl_450` each produced non-empty outputs and passed.

**Expected Behavior**
- All core targets compile from the same compact source.

**Conclusion**
- Current baseline target matrix for compute is healthy and suitable as a stable smoke gate.

**Action Items**
- [x] Keep this matrix in smoke tests.
- [ ] Add additional graphics-stage smoke coverage (vertex + fragment) using compact shaders.

---

### 2026-02-13 — Graphics-stage corpus should use tiny focused shaders
**Context**
- Expand corpus for basic graphics stages without increasing test complexity.

**Command(s)**
```powershell
# Added compact test shaders under Tests/Slang.Sdk.Tests/Shaders
# and validated via dotnet test (RuntimeAndCli + InteropBindingPretty)
```

**Observed Behavior**
- Minimal single-purpose shaders (<50 LOC) are sufficient for stage-specific compile checks.
- Keeping one entry point per file simplifies diagnostics and failure triage.

**Expected Behavior**
- Fast, deterministic tests with clear failure attribution by stage/file.

**Conclusion**
- Use stage-focused files for smoke tests; reserve multi-entry shaders for integration coverage.

**Action Items**
- [x] Add `VertexTransformMinimal.slang` and `FragmentSolidColorMinimal.slang`.
- [x] Add smoke validation that compiles these shaders on core graphics targets.

---

### 2026-02-13 — Runtime-native coupling is explicit and testable
**Context**
- Confirm test suite catches runtime packaging regressions early.

**Command(s)**
```powershell
dotnet test Tests/Slang.Sdk.Tests/Slang.Sdk.Tests.csproj --filter "RuntimeDirectory_ShouldExist_AndContainNativeTools"
```

**Observed Behavior**
- Tests fail clearly when `slangc.exe` or `SlangNative.dll` is unavailable from runtime directory resolution.

**Expected Behavior**
- Packaging/runtime path issues should fail fast with actionable messages.

**Conclusion**
- Keep runtime existence checks as mandatory smoke prerequisites before deeper integration tests.

**Action Items**
- [x] Maintain runtime-path smoke assertion.
- [ ] Extend assertions later for additional required dependencies if packaging regressions recur.

---

### 2026-02-13 — Texture/sampler fixture compiles cross-target when entry stage is explicit
**Context**
- Add compact fragment coverage for practical resource binding (`Texture2D` + `SamplerState`) in smoke/integration tests.

**Command(s)**
```powershell
dotnet test Tests/Slang.Sdk.Tests/Slang.Sdk.Tests.csproj --filter "FragmentTextureSamplerMinimal|TextureSampler"
```

**Observed Behavior**
- HLSL (`sm_5_0`) and GLSL (`glsl_450`) compile cleanly when `entry=FS` and `stage=fragment` are both set.

**Expected Behavior**
- Deterministic output for both CLI and Pretty API compile paths.

**Conclusion**
- Keep explicit entry+stage in tests; avoids ambiguous defaults in mixed shader corpora.

**Action Items**
- [x] Add `FragmentTextureSamplerMinimal.slang` fixture.
- [x] Cover via smoke and integration compile checks.

---

### 2026-02-13 — Struct semantic pass-through is a useful low-cost vertex regression guard
**Context**
- Expand vertex coverage with semantic carry-through (`POSITION` + `COLOR0`) using a tiny struct-based shader.

**Command(s)**
```powershell
dotnet test Tests/Slang.Sdk.Tests/Slang.Sdk.Tests.csproj --filter "VertexSemanticStructMinimal|SemanticStruct"
```

**Observed Behavior**
- `VSIn -> VSOut` semantic mapping remains stable on HLSL/GLSL in both CLI and Pretty compile paths.

**Expected Behavior**
- No target-specific failures for straightforward semantic forwarding.

**Conclusion**
- This fixture provides meaningful graphics API coverage without increasing test complexity.

**Action Items**
- [x] Add `VertexSemanticStructMinimal.slang` fixture.
- [x] Include in target-matrix smoke and integration checks.

---

## Notes
- Prefer minimal, reproducible command lines.
- For shader experiments, keep files simple (target < 50 lines) and focused.
- Tag target/API in titles when relevant (HLSL/GLSL/SPIR-V, reflection, diagnostics, etc.).

---

## Change Log
- YYYY-MM-DD: Created template.
- 2026-02-13: Added findings for target matrix behavior, compact graphics shader strategy, and runtime-native coupling checks.
- 2026-02-13: Added texture/sampler and struct-semantic fixture observations plus edge-case guidance for explicit stage/entry usage.
