# CS2TS-Models portfolio roadmap

## Snapshot
- 90-day evidence: 5 commits, 35 files changed
- Last signal: `be69b5a`
- Top modified areas: `typescript`, `docs`, `tests`
- Stack: .NET + TypeScript
- Docs folder: yes
- Roadmap folder: no
- Features docs: yes
- Tests indexed: yes

## Implemented now (V1 baseline)
- Core generation logic is documented in `cs2ts-core-logic.md` and project layout in `net-core-project-structure.md`.
- TypeScript transformation and typing concerns are covered in `typescript-strong-typing.md` and `typescript-implementation.md`.
- CLI and environment configuration is documented in `node-js-runtime-environment.md` and `node-js-environment-configuration.md`.
- Fixtures and tests are now present in `CS2TS.Tests` and `CS2TS.Tests.Fixtures`.

## Gaps identified
- End-to-end contract validation between C# models and generated TS output can be strengthened.
- Test-to-feature ownership is not yet fully explicit.
- Build/release assumptions across dual runtime (dotnet + node) paths need explicit lockstep checks.

## V1 (stability)
- [ ] Add explicit contract tests for fixture-to-output generation parity.
- [ ] Add CI gate for both dotnet and Node compile/test paths.
- [ ] Document failure handling for output verification mismatches.
- [ ] Add clear release steps for generated artifacts.

## V2 (confidence)
- [ ] Add deterministic snapshot checks for generated TS model outputs.
- [ ] Add environment matrix for Node/SDK compatibility.
- [ ] Improve changelog and migration guidance for CLI option changes.
- [ ] Add runbook for dependency updates affecting both runtimes.

## V10 (scale)
- [ ] Introduce long-term contract and versioning policy for generated type surface.
- [ ] Add automated diff gating and backward-compatibility checks.
- [ ] Build plugin-style extension points for custom naming/type strategies.
- [ ] Publish a deprecation and migration policy for generated output changes.

## Readiness checklist
- [ ] Generator and fixture tests pass in both runtimes.
- [ ] Output contract documented and versioned before release.
- [ ] Any CLI change includes docs and migration note.
