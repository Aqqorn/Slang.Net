# Missing Features vs `slang.h`

> Purpose: Track high-impact API gaps between upstream `slang.h` and current Slang.Sdk surface with symbol-level mapping.

## References
- Upstream header: `src/Native/EmbeddedLLVM/slang-2025.13.2-windows/x64/include/slang.h`
- Native shim: `src/Native/`
- Interop layer: `src/Slang.Sdk/Layer1-Interop/`
- Binding layer: `src/Slang.Sdk/Layer2-Binding/`
- Pretty layer: `src/Slang.Sdk/Layer3-Pretty/`

## Status Legend
- ✅ Implemented
- 🟡 Partial
- ❌ Missing
- ⚪ Deferred / intentionally omitted

---

## Symbol-Level Coverage Map (`IGlobalSession` / `ISession` / `IComponentType` / `IModule`)

| `slang.h` symbol | Status | Native shim / Interop mapping | Binding / Pretty mapping | Notes |
|---|---|---|---|---|
| `IGlobalSession::createSession` | 🟡 Partial | `Session_Create` (`SlangNativeInterop`) | `Binding.Session` ctor, `Session.Builder.Create()` | Session creation works; full `IGlobalSession` object not exposed as first-class managed type. |
| `IGlobalSession::findProfile` | ✅ Implemented | `GlobalSession_FindProfile` | `Session.FindProfile(string)` | Returns native `SlangProfileID` (0 when not found). |
| `IGlobalSession::checkCompileTargetSupport` | ✅ Implemented | `GlobalSession_CheckCompileTargetSupport` | `Session.CheckCompileTargetSupport(Target.CompileTarget)` | Returns `SlangResult` for target support probing. |
| `IGlobalSession::checkPassThroughSupport` | ✅ Implemented | `GlobalSession_CheckPassThroughSupport` | `Session.CheckPassThroughSupport(PassThrough)` | Pass-through availability probe is now surfaced. |
| `IGlobalSession::findCapability` | ✅ Implemented | `GlobalSession_FindCapability` | `Session.FindCapability(string)` | Returns native `SlangCapabilityID` (0 when not found). |
| `IGlobalSession::parseCommandLineArguments` | ❌ Missing | — | — | No managed `SessionDesc` parse path from slangc-style args. |
| `IGlobalSession::createCompileRequest` (deprecated) | 🟡 Partial | CompileRequest wrappers exist (`CompileRequest_*`) | `Binding.CompileRequest` (legacy path) | Legacy compile-request API exists but not parity-complete with modern component workflows. |
| `ISession::loadModule` | ✅ Implemented | `Module_Import` | `Session.ImportModule()` / `new Binding.Module(session,name)` | Import-by-name path exists; native shim now composes+links module entry points so `Module.Program` compile flows work for imported modules. |
| `ISession::loadModuleFromSource` | 🟡 Partial | `Module_Create(parent,name,path,source)` | `Session.LoadModule(...)` | Implemented via custom shim helper, not direct `ISession` method exposure. |
| `ISession::loadModuleFromSourceString` | 🟡 Partial | `Module_Create(parent,name,path,source)` | `Session.LoadModuleFromSourceString(name, sourceText, modulePath?)` | Explicit pretty/binding in-memory load now exposed via existing shim helper (not a direct `ISession` native call). |
| `ISession::loadModuleFromIRBlob` | ❌ Missing | — | — | No IR blob load support. |
| `ISession::createCompositeComponentType` | ❌ Missing | — | — | No explicit component composition surface. |
| `ISession::createTypeConformanceComponentType` | ❌ Missing | — | — | Dynamic dispatch conformance control unavailable. |
| `ISession::getLoadedModuleCount` | 🟡 Partial | `Session_GetModuleCount` | `Binding.Session.GetModuleCount()` | Available in binding, not clearly surfaced as modern session semantics. |
| `ISession::getLoadedModule` | 🟡 Partial | `Session_GetModuleByIndex` | `Binding.Session.GetModuleByIndex()` | Available in binding, no broad pretty collection integration from runtime session state. |
| `ISession::getTypeLayout` / `specializeType` / RTTI helpers | ❌ Missing | — | — | Reflection APIs exist post-compile, but session-level type utilities not exposed. |
| `IComponentType::getLayout` | ❌ Missing | — | — | No direct layout call on component/program/module wrappers. |
| `IComponentType::specialize` | ❌ Missing | — | — | No specialization arg API exposed. |
| `IComponentType::link` | ❌ Missing | — | — | Link step not first-class; compile flows rely on custom module/program wrappers. |
| `IComponentType::linkWithOptions` | ❌ Missing | — | — | No link-time compiler options path. |
| `IComponentType::getTargetCode` | ❌ Missing | — | — | No direct target-code accessor on component types. |
| `IComponentType::getEntryPointCode` | 🟡 Partial | `Program_CompileEntryPoint`, `EntryPoint_Compile` | `ProgramTarget.EntryPoints[name].Compile()` | Equivalent behavior exists through custom wrappers, not direct `IComponentType` contract. |
| `IComponentType::getResultAsFileSystem` | ❌ Missing | — | — | No in-memory file-system output API. |
| `IComponentType::getEntryPointHash` | ❌ Missing | — | — | Cache key/hash surface absent. |
| `IComponentType::getTargetMetadata` / `getEntryPointMetadata` | ❌ Missing | — | — | Metadata interface (`IMetadata`) not surfaced. |
| `IComponentType::renameEntryPoint` | ❌ Missing | — | — | No renaming facility. |
| `IModule::findEntryPointByName` | ✅ Implemented | `Module_FindEntryPointByName` | `Binding.Module` / entry point wrappers | Covered via module and reflection entry-point flows. |
| `IModule::getDefinedEntryPointCount` / `getDefinedEntryPoint` | 🟡 Partial | `Module_GetEntryPointCount`, `Module_GetEntryPointByIndex` | Used by reflection/entrypoint abstractions | Implemented through shim naming; semantics aligned to defined entry points for current flow. |
| `IModule::getName` | ✅ Implemented | `Module_GetName` | `Binding.Module.Name` | Directly available. |
| `IModule::serialize` | ❌ Missing | — | — | Binary module serialization unavailable. |
| `IModule::writeToFile` | ❌ Missing | — | — | No module blob write-through API. |
| `IModule::getFilePath` / `getUniqueIdentity` | ❌ Missing | — | — | Module identity/path metadata missing. |
| `IModule::findAndCheckEntryPoint` | ❌ Missing | — | — | Stage-aware entry-point validation not exposed. |
| `IModule::getDependencyFileCount` / `getDependencyFilePath` | ❌ Missing | — | — | Dependency tracking absent. |
| `IModule::disassemble` | ❌ Missing | — | — | No disassembly surface. |

---

## High-Impact Gaps (prioritized)

### High
- [x] Add `ISession::loadModuleFromSourceString` managed surface + tests (implemented via existing module-create shim path).
- [ ] Add `IComponentType::link` + `getTargetCode` minimal pipeline.
- [ ] Add `IModule::serialize` + `disassemble` + dependency queries.

### Medium
- [x] Add `IGlobalSession` capability probes: `findProfile`, `findCapability`, `checkCompileTargetSupport`, `checkPassThroughSupport`.
- [ ] Add metadata/hash APIs: `getEntryPointHash`, `getTargetMetadata`, `getEntryPointMetadata`.
- [ ] Add `ISession::loadModuleFromIRBlob` for binary module workflows.

### Low
- [ ] Evaluate `IModulePrecompileService_Experimental` once core component APIs are covered.

---

## Actionable Implementation Slices (next batches)

### Batch A — Global capability probes (low-risk / high leverage)
- **Symbols:** `IGlobalSession::findProfile`, `findCapability`, `checkCompileTargetSupport`, `checkPassThroughSupport`.
- **Expected touchpoints:**
  - `src/Native/*.cpp|*.h`: add C exports for each probe against global session.
  - `src/Slang.Sdk/Layer1-Interop/SlangNativeInterop.cs`: new P/Invoke declarations.
  - `src/Slang.Sdk/Layer1-Interop/StrongInterop*.cs`: safe wrappers + enum/value conversions.
  - `src/Slang.Sdk/Layer3-Pretty/Session*.cs` (or `Runtime` helper): user-facing probe APIs.
- **Test candidates:**
  - Smoke: `RuntimeAndCliTests` target-matrix can replace hard-coded assumptions with probe-guarded asserts.
  - Integration: verify probe results for HLSL/GLSL/SPIR-V are internally consistent with compile success.

### Batch B — In-memory module loading and composition primitives
- **Symbols:** `ISession::loadModuleFromSourceString`, `createCompositeComponentType` (minimal), `IComponentType::link`.
- **Expected touchpoints:**
  - `src/Native`: export module-from-string + compose + link helpers.
  - `Layer1-Interop`: map new handles/return diagnostics.
  - `Layer2-Binding/Session*` + `Program*`: add managed constructors/compose APIs.
  - `Layer3-Pretty`: small ergonomic overloads (`LoadModule(name, sourceText)`, `Program.Link()`).
- **Test candidates:**
  - Integration: create source string in-test, compile/link to HLSL and GLSL.
  - Negative test: malformed source string returns diagnostics.

### Batch C — Component output extraction and module serialization
- **Symbols:** `IComponentType::getTargetCode`, `IModule::serialize`, `IModule::disassemble`, dependency file queries.
- **Expected touchpoints:**
  - `src/Native`: byte-blob/file-system bridging exports.
  - `Layer1-Interop`: blob marshaling helpers.
  - `Layer2-Binding`: typed result wrappers (code blob + metadata).
  - `Layer3-Pretty`: simple `CompileToBytes`, `Serialize`, `Disassemble` conveniences.
- **Test candidates:**
  - Smoke: non-empty byte payload for at least one target.
  - Integration: serialize->reload (when IR load slice lands), disassemble returns non-empty text.

## Implementation Criteria
Each new symbol should ship with:
1. Native shim export in `src/Native`.
2. Interop declaration in `SlangNativeInterop` + `StrongInterop` wrapper.
3. Binding + Pretty exposure where appropriate.
4. Smoke or integration test (CLI and/or API path).
5. Coverage update in this file.

---

## Change Log
- 2026-02-13: Replaced template with symbol-level mapping against `slang.h` and current Layer1/2/3 wrappers.
- 2026-02-13: Added implementation-slice batches (A/B/C) with expected files, touchpoints, and test candidates.
- 2026-02-13: Implemented Batch A global capability probes end-to-end (`findProfile`, `findCapability`, `checkCompileTargetSupport`, `checkPassThroughSupport`) across Native + Interop + Binding + Pretty + smoke tests.
- 2026-02-13: Added Batch B slice: explicit managed `Session.LoadModuleFromSourceString(...)` in Binding/Pretty with integration tests for inline compile success and invalid-source diagnostics.
- 2026-02-13: Improved `ISession::loadModule` shim behavior to compose+link imported modules into a program component; added integration coverage for `Session.ImportModule(...).Program` target compilation (with stale-native compatibility guard).
