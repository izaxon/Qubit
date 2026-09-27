# Changelog

## Unreleased

### Added

- An immutable `QuantumRegister` state vector for 1–16 qubits, with indexed
  X, Y, Z, Hadamard, rotation, phase-shift, and CNOT operations.
- Computational-basis measurement with probabilities, sampled outcomes, and
  collapsed post-measurement states. A supplied `Random` enables repeatable
  simulations.
- Conversion between single-, two-, and multi-qubit representations.
- `PhysicallyEquivalentTo` for tolerance-based comparison up to global phase,
  while `Equals` retains exact amplitude comparison.
- Tests for composed gates, Bell correlations, measurement, inverse operations,
  and invalid inputs, plus GitHub Actions verification for pushes and PRs.

## 2026-09-27

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
