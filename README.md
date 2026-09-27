# Qubit

Qubit is a small C# library for experimenting with quantum gates. It represents
single-qubit states and two-qubit states, including entangled states produced by
a controlled NOT (CNOT) gate. The project originated on CodePlex and was
imported to GitHub in 2016.

## Requirements

- .NET 10 SDK

The library and the verification program have no third-party package dependencies.

## Build and verify

```sh
dotnet build Qubit.sln
dotnet run --project TestQubit/TestQubit.csproj
```

The verification program exits with an error if a check fails. It covers the
single-qubit gates, CNOT basis states, entanglement, invalid state amplitudes,
and the gate wrappers.

## Use the library

Add a project reference to `Qubit/Qubit.csproj`, then use the `Qubit` namespace:

```csharp
using System;
using Qubit;

qubit zero = qubit.Zero;                // |0>
qubit plus = zero.Hadamard();           // (|0> + |1>) / sqrt(2)
qubit rotated = zero.Rotate(Math.PI/4); // Same amplitudes for this input
TwoQubitState bell = qubit.CNOT(plus, zero);

Console.WriteLine(bell.ZeroZero); // 1 / sqrt(2)
Console.WriteLine(bell.OneOne);   // 1 / sqrt(2)
```

`qubit` stores the complex amplitudes `Alpha` for `|0>` and `Beta` for
`|1>`. It provides `Not`/`PauliX`, `PauliY`, `PauliZ`, `Hadamard`,
`Rotate`, and `PhaseShift`. States are immutable and constructors require
finite, normalized amplitudes.

`TwoQubitState` stores amplitudes in the order `|00>`, `|01>`, `|10>`,
`|11>`. Use `TwoQubitState.FromProduct(first, second)` for independent
inputs, or its constructor to represent an entangled state. `CNOT()` uses
the first qubit as control and the second as target. A CNOT result may be
entangled, so it cannot generally be reduced to two independent `qubit`
objects. `Gates.ControlledNot.Update()` therefore exposes `OutputState`.

Amplitudes are compared exactly by `Equals`; use a numerical tolerance when
checking results that involve trigonometric functions.

This library is intended for learning and small examples. It currently has no
measurement API or general multi-qubit circuit model.

## Background

The original work was inspired by Leonard Susskind and Michael Nielsen.
See [Quantum Computing for the Determined](https://michaelnielsen.org/blog/quantum-computing-for-the-determined/).
