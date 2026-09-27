# Qubit

Qubit is a small, educational C# state-vector simulator. It supports single
qubits and registers of up to 16 qubits, including entangled states. The project
originated on CodePlex and was imported to GitHub in 2016.

## Requirements

- .NET 10 SDK

There are no third-party package dependencies.

## Build and verify

```sh
dotnet build Qubit.sln --configuration Release
dotnet run --project TestQubit/TestQubit.csproj --configuration Release --no-build
```

The verification program returns an error if a check fails. GitHub Actions
runs the same commands for pull requests and pushes to `master`.

## Single qubits

`qubit` stores complex amplitudes `Alpha` and `Beta` for `|0>` and
`|1>`. It provides X, Y, Z, Hadamard, real rotation, and phase-shift
operations. States are immutable and must have finite amplitudes whose squared
magnitudes sum to one.

```csharp
using Qubit;

qubit plus = qubit.Zero.Hadamard(); // (|0> + |1>) / sqrt(2)
qubit minus = qubit.One.Hadamard();
```

The `Rotate(angle)` and `ApplyRotation(target, angle)` methods use the
matrix `[cos(angle) -sin(angle); sin(angle) cos(angle)]`, corresponding to a
Bloch-sphere `Ry(2 * angle)` rotation.

## Registers and gates

`QuantumRegister` holds `2^n` complex amplitudes for `n` qubits. Qubit
index 0 is the leftmost bit in the basis label: for two qubits the amplitude
order is `|00>`, `|01>`, `|10>`, `|11>`. Each gate returns a new register.

```csharp
using Qubit;

QuantumRegister bell = QuantumRegister.Zero(2)
    .ApplyHadamard(0)
    .ApplyCNOT(control: 0, target: 1);

// (|00> + |11>) / sqrt(2)
System.Console.WriteLine(bell.Amplitude(0));
System.Console.WriteLine(bell.Amplitude(3));

// Gates can still act on either qubit after entanglement.
QuantumRegister changed = bell.ApplyHadamard(1);
```

Indexed gates include `ApplyX`, `ApplyY`, `ApplyZ`, `ApplyHadamard`,
`ApplyRotation`, `ApplyPhaseShift`, and `ApplyCNOT`. Use
`QuantumRegister.FromProduct(qubits)` to combine independent qubits.
`qubit.ToRegister()` and `TwoQubitState.ToRegister()` bridge the original
APIs to the register model.

## Probabilities and measurement

`ProbabilityOf(target, outcome)` returns the probability for outcome 0 or 1
in the computational basis. `Measure(target)` samples an outcome and returns
the bit, its probability, and a normalized collapsed state. Pass a
`System.Random` instance to repeat a simulation.

```csharp
var result = bell.Measure(target: 0, random: new System.Random(42));
System.Console.WriteLine(result.Outcome);     // 0 or 1
System.Console.WriteLine(result.Probability); // 0.5 for this Bell state

// The other qubit is now correlated with the measured bit.
System.Console.WriteLine(result.State.ProbabilityOf(1, result.Outcome)); // 1
```

Gates evolve amplitudes deterministically. Sampling is confined to
`Measure`; use `ProbabilityOf` when you need the exact probabilities without
changing the state.

## Comparing states

`Equals` compares amplitudes exactly. This is useful for exact copies but
rarely appropriate after floating-point calculations. Use
`PhysicallyEquivalentTo(other, tolerance)` to compare pure states within a
tolerance while ignoring a common global phase:

```csharp
bool sameState = qubit.Zero.PhysicallyEquivalentTo(new qubit(-1, 0)); // true
bool sameAmplitudes = qubit.Zero.Equals(new qubit(-1, 0));             // false
```

A relative phase can change interference and is therefore retained by
`PhysicallyEquivalentTo`.

## Scope

This is a small state-vector learning project. Memory grows as `2^n`, so the
library limits registers to 16 qubits. It supports pure states, the listed
gates, and computational-basis measurements. It does not model noise,
mixed states, hardware execution, or arbitrary circuits.

The original work was inspired by Leonard Susskind and Michael Nielsen.
See [Quantum Computing for the Determined](https://michaelnielsen.org/blog/quantum-computing-for-the-determined/).
