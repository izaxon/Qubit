# Changelog

## Unreleased — 2026-09-27

### Changed

- Target .NET 10 with SDK-style projects and a current solution file.
- Make single-qubit states immutable and validate their normalization in all builds.
- **Breaking:** `qubit.CNOT(control, target)` returns `TwoQubitState` instead of
  `qubit`. Callers can read the four basis amplitudes or apply another CNOT
  to that state.
- **Breaking:** `Gates.ControlledNot` exposes `OutputState` instead of
  `OutputTarget`, because CNOT can produce an entangled state.
- **Breaking:** `qubit.Alpha` and `qubit.Beta` are read-only, and gate pin
  values are nullable until assigned.

### Fixed

- Apply the rotation matrix to both input amplitudes correctly.
- Apply the Pauli Y matrix with the correct amplitude swap and phases.
- Preserve all four amplitudes through CNOT, including entangled results.
- Reject zero, non-normalized, and non-finite quantum states.
- Handle null and unrelated values safely in equality checks; provide matching
  hash codes for value equality.
- Format a zero complex amplitude as `0`.

### Added

- A package-free executable verification program covering gates, entanglement,
  invalid states, and gate wrappers.
- Build and usage instructions in the README.
