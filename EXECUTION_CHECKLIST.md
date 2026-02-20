# Slang.Sdk Execution Checklist (Composer)

> Planning-only checklist. No implementation started yet.

## Ground Rules (from Aqqorn)
- [ ] Before coding: create branch `composer` from `development`
- [ ] Treat `Tests/AttributeMemoryLeakTest` as deprecated
- [ ] Build native first:
  - `cd src`
  - `.\all-platforms.ps1 -script Native\build.ps1`
- [ ] Then build .NET project (native changes should flow through automatically)
- [ ] Keep test shaders simple (target < 50 lines each), but comprehensive
- [ ] Validate different graphics APIs supported by Slang
- [ ] Inspect `source/Header References for Slang.Net/include/slang.h` for missing features
- [ ] Use `slangc` on-machine for quick behavior validation

---

## Milestone M1 — Test Harness Modernization

### 1. Baseline & inventory
- [ ] Inventory all active tests under `src/Slang.Sdk/Tests`
- [ ] Classify test types: interop, binding, pretty API, CLI
- [ ] Identify tests that are interactive/non-CI-safe
- [ ] Define pass/fail criteria for each class

### 2. Convert to CI-friendly test shape
- [ ] Replace console-runner style with formal test project(s) (xUnit preferred)
- [ ] Remove blocking interactivity (`Console.ReadKey` etc.)
- [ ] Add traits/categories (Fast, Integration, NativeDependent, CLI)
- [ ] Make test output machine-readable in CI

### 3. Deterministic test assets
- [ ] Create `tests/shaders/` corpus with small focused shaders (<50 lines each)
- [ ] Ensure file paths are deterministic and copied to output reliably
- [ ] Add helper APIs for stable session/module setup/teardown

### 4. CI bootstrap
- [ ] Add workflow to build native first, then run managed tests
- [ ] Add matrix knobs (Debug/Release; platform where feasible)
- [ ] Publish logs/artifacts on failure

---

## Milestone M2 — Coverage Matrix for Graphics Basics

## Shader corpus rules
- [ ] Keep each shader under ~50 lines
- [ ] One concept per file (minimal but explicit)
- [ ] Prefer readable names and comments for intent

## Basic graphics concepts to cover
- [ ] Vertex input/output semantics
- [ ] Pixel shader color output
- [ ] Compute entry point and thread group layout
- [ ] Constant buffer / parameter binding basics
- [ ] Texture + sampler usage
- [ ] Structured/resource binding and reflection visibility
- [ ] Multiple entry points per module
- [ ] Minimal error-case shader (for diagnostics path)

## API target coverage (compile-time checks)
- [ ] HLSL target(s)
- [ ] GLSL target(s)
- [ ] SPIR-V target(s)
- [ ] Any other targets currently exposed by Slang.Sdk API surface

## Validation style
- [ ] Validate compile succeeds/fails as expected per target
- [ ] Validate reflection contains expected parameters/entrypoints
- [ ] Validate diagnostics text for expected failure cases

---

## Milestone M3 — Missing Feature Gap Analysis (`slang.h`)

### 1. API surface comparison
- [ ] Enumerate relevant symbols from `slang.h`
- [ ] Map against Layer1 interop coverage (`StrongInterop.*`, native interop declarations)
- [ ] Mark status: Implemented / Partial / Missing / Intentionally Omitted

### 2. Prioritization
- [ ] Rank gaps by impact: blocker, high, medium, low
- [ ] Identify dependencies/risk for each missing item
- [ ] Define first implementation batch (small + high impact)

### 3. Deliverable
- [ ] Create `docs/MISSING_FEATURES.md` with clear checklist and owners

---

## Milestone M4 — Bug Burn-Down Workflow

- [ ] Create bug template requiring reproducible steps
- [ ] For every bug: add failing regression test first
- [ ] Fix bug in smallest possible patch
- [ ] Verify pass on target matrix and no regressions
- [ ] Maintain rolling Known Issues + Fixed log

---

## Milestone M5 — Samples & Tutorial Reliability

- [ ] Confirm active sample set (`CLInvoke`, `CompilationAPI`, `ReflectionAPI`, `SlangCube`)
- [ ] Define learning path order for docs
- [ ] Validate each sample build/run behavior with current package output
- [ ] Add sample verification step in CI where practical
- [ ] Remove/replace stale sample references in docs

---

## Milestone M6 — Learning Notes / Research Log

- [ ] Keep a running log in `docs/LEARNINGS.md`
- [ ] Capture `slangc` command experiments and observations
- [ ] Note target-specific quirks and workarounds
- [ ] Link findings back to tests to avoid rediscovering issues

---

## Ready-to-start sequence (when approved)
1. [ ] `git checkout development`
2. [ ] `git pull`
3. [ ] `git checkout -b composer`
4. [ ] Native build (`cd src; .\all-platforms.ps1 -script Native\build.ps1`)
5. [ ] Baseline managed build + baseline test run
6. [ ] Begin M1 conversion in small PR-sized slices
