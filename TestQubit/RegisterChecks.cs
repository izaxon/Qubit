using System;
using Qubit;

internal static class RegisterChecks
{
    private const double Tolerance = 1e-10;
    private static int checks;

    public static int Run()
    {
        checks = 0;
        CheckState(QuantumRegister.Zero(3), 1, 0, 0, 0, 0, 0, 0, 0);
        CheckState(QuantumRegister.Zero(3).ApplyX(1), 0, 0, 1, 0, 0, 0, 0, 0);
        CheckState(QuantumRegister.Zero(3).ApplyX(0).ApplyX(2),
            0, 0, 0, 0, 0, 1, 0, 0);
        CheckState(QuantumRegister.Zero(2).ApplyHadamard(0),
            1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2), 0);
        CheckState(QuantumRegister.Zero(2).ApplyHadamard(1),
            1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0, 0);
        CheckState(QuantumRegister.Zero(2).ApplyY(1),
            0, complex.I, 0, 0);
        CheckState(QuantumRegister.Zero(2).ApplyX(1).ApplyZ(1),
            0, -1, 0, 0);
        CheckState(QuantumRegister.Zero(2).ApplyRotation(0, Math.PI / 2),
            0, 0, 1, 0);
        CheckState(QuantumRegister.Zero(2).ApplyX(1).ApplyPhaseShift(1, 0, Math.PI / 2),
            0, complex.I, 0, 0);
        CheckState(QuantumRegister.Zero(3).ApplyX(0).ApplyCNOT(0, 2),
            0, 0, 0, 0, 0, 1, 0, 0);
        CheckCnotTruthTable();

        var bell = QuantumRegister.Zero(2).ApplyHadamard(0).ApplyCNOT(0, 1);
        CheckState(bell, 1 / Math.Sqrt(2), 0, 0, 1 / Math.Sqrt(2));
        CheckState(bell.ApplyHadamard(0), 0.5, 0.5, 0.5, -0.5);
        CheckState(bell.ApplyCNOT(0, 1),
            1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2), 0);
        Check(bell.ApplyHadamard(1).ApplyHadamard(1).PhysicallyEquivalentTo(bell),
            "Hadamard squared");
        Check(bell.ApplyX(0).ApplyX(0).Equals(bell), "X squared");
        Check(bell.ApplyY(1).ApplyY(1).PhysicallyEquivalentTo(bell), "Y squared");
        Check(bell.ApplyZ(0).ApplyZ(0).Equals(bell), "Z squared");
        Check(bell.ApplyCNOT(0, 1).ApplyCNOT(0, 1).Equals(bell), "CNOT squared");
        Check(bell.ApplyRotation(0, 0.37).ApplyRotation(0, -0.37)
            .PhysicallyEquivalentTo(bell), "Inverse rotation");
        Check(bell.ApplyPhaseShift(1, 0.3, -0.7)
            .ApplyPhaseShift(1, -0.3, 0.7)
            .PhysicallyEquivalentTo(bell), "Inverse phase shift");

        var source = new complex[] { 1, 0 };
        var copied = new QuantumRegister(1, source);
        source[0] = 0;
        var returned = copied.ToArray();
        returned[0] = 0;
        CheckAmplitude(copied.Amplitude(0), 1);
        Check(QuantumRegister.FromProduct(qubit.One, qubit.Zero)
            .Equals(QuantumRegister.Zero(2).ApplyX(0)), "Product ordering");
        Check(TwoQubitState.FromRegister(bell).ToRegister().Equals(bell),
            "Two-qubit bridge");
        Check(qubit.Zero.ToRegister().Equals(QuantumRegister.Zero(1)),
            "Single-qubit bridge");

        Throws<ArgumentOutOfRangeException>(() => QuantumRegister.Zero(0));
        Throws<ArgumentOutOfRangeException>(() => QuantumRegister.Zero(QuantumRegister.MaxQubits + 1));
        Throws<ArgumentException>(() => new QuantumRegister(2, 1, 0));
        Throws<ArgumentException>(() => new QuantumRegister(1, 0, 0));
        Throws<ArgumentException>(() => new QuantumRegister(1, new complex(double.NaN), 0));
        Throws<ArgumentOutOfRangeException>(() => bell.Amplitude(4));
        Throws<ArgumentOutOfRangeException>(() => bell.ApplyX(2));
        Throws<ArgumentOutOfRangeException>(() => bell.ApplyCNOT(-1, 1));
        Throws<ArgumentException>(() => bell.ApplyCNOT(0, 0));
        Throws<ArgumentOutOfRangeException>(() => bell.ProbabilityOf(0, 2));
        Throws<ArgumentException>(() => TwoQubitState.FromRegister(QuantumRegister.Zero(1)));

        MeasurementChecks(bell);
        PhaseChecks(bell);
        return checks;
    }

    private static void MeasurementChecks(QuantumRegister bell)
    {
        CheckClose(bell.ProbabilityOf(0, 0), 0.5);
        CheckClose(bell.ProbabilityOf(0, 1), 0.5);
        CheckClose(bell.ProbabilityOf(1, 0), 0.5);
        CheckClose(bell.ProbabilityOf(1, 1), 0.5);

        var firstZero = bell.Measure(0, new FixedRandom(0.25));
        Check(firstZero.Outcome == 0, "Bell first measurement: zero");
        CheckClose(firstZero.Probability, 0.5);
        CheckState(firstZero.State, 1, 0, 0, 0);
        Check(firstZero.State.Measure(1, new FixedRandom(0.75)).Outcome == 0,
            "Bell correlation after zero");

        var firstOne = bell.Measure(0, new FixedRandom(0.75));
        Check(firstOne.Outcome == 1, "Bell first measurement: one");
        CheckClose(firstOne.Probability, 0.5);
        CheckState(firstOne.State, 0, 0, 0, 1);
        Check(firstOne.State.Measure(1, new FixedRandom(0.25)).Outcome == 1,
            "Bell correlation after one");

        var secondOne = bell.Measure(1, new FixedRandom(0.75));
        Check(secondOne.Outcome == 1, "Measure target qubit");
        CheckState(secondOne.State, 0, 0, 0, 1);

        var product = QuantumRegister.Zero(2).ApplyHadamard(0).ApplyHadamard(1);
        var measured = product.Measure(0, new FixedRandom(0.25));
        CheckState(measured.State, 1 / Math.Sqrt(2), 1 / Math.Sqrt(2), 0, 0);
        CheckClose(measured.State.ProbabilityOf(1, 0), 0.5);
        CheckState(QuantumRegister.Zero(2).Measure(0, new FixedRandom(0)).State,
            1, 0, 0, 0);
    }

    private static void PhaseChecks(QuantumRegister bell)
    {
        var negated = new QuantumRegister(2,
            -bell.Amplitude(0), -bell.Amplitude(1),
            -bell.Amplitude(2), -bell.Amplitude(3));
        var imaginary = new QuantumRegister(2,
            complex.I * bell.Amplitude(0), complex.I * bell.Amplitude(1),
            complex.I * bell.Amplitude(2), complex.I * bell.Amplitude(3));
        Check(!bell.Equals(negated), "Exact amplitude equality retains phase");
        Check(bell.PhysicallyEquivalentTo(negated), "Negative global phase");
        Check(bell.PhysicallyEquivalentTo(imaginary), "Imaginary global phase");
        Check(TwoQubitState.FromRegister(bell)
            .PhysicallyEquivalentTo(TwoQubitState.FromRegister(negated)),
            "Two-qubit physical equivalence");
        Check(qubit.Zero.PhysicallyEquivalentTo(new qubit(-1, 0)),
            "Single-qubit physical equivalence");
        Check(!qubit.Zero.Hadamard().PhysicallyEquivalentTo(qubit.One.Hadamard()),
            "Relative phase differs");
        Check(!bell.PhysicallyEquivalentTo(QuantumRegister.Zero(2)),
            "Distinct physical states");
        Check(!qubit.Zero.PhysicallyEquivalentTo(qubit.One),
            "Orthogonal states are not phase-equivalent");
        Check(!bell.PhysicallyEquivalentTo(QuantumRegister.Zero(1)),
            "Different register sizes");
        Throws<ArgumentOutOfRangeException>(() => bell.PhysicallyEquivalentTo(bell, -1));
    }

    private static void CheckCnotTruthTable()
    {
        var cases = new (int Control, int Target, int[] Outputs)[]
        {
            (0, 1, new[] { 0, 1, 2, 3, 6, 7, 4, 5 }),
            (0, 2, new[] { 0, 1, 2, 3, 5, 4, 7, 6 }),
            (1, 0, new[] { 0, 1, 6, 7, 4, 5, 2, 3 }),
            (1, 2, new[] { 0, 1, 3, 2, 4, 5, 7, 6 }),
            (2, 0, new[] { 0, 5, 2, 7, 4, 1, 6, 3 }),
            (2, 1, new[] { 0, 3, 2, 1, 4, 7, 6, 5 })
        };
        foreach (var (control, target, outputs) in cases)
        {
            for (int basis = 0; basis < 8; basis++)
            {
                var amplitudes = new complex[8];
                amplitudes[basis] = 1;
                var result = new QuantumRegister(3, amplitudes)
                    .ApplyCNOT(control, target);
                for (int index = 0; index < 8; index++)
                {
                    CheckAmplitude(result.Amplitude(index),
                        index == outputs[basis] ? 1 : 0);
                }
            }
        }
    }

    private static void CheckState(QuantumRegister actual, params complex[] expected)
    {
        Check(actual.BasisStateCount == expected.Length, "Basis state count");
        for (int i = 0; i < expected.Length; i++)
        {
            CheckAmplitude(actual.Amplitude(i), expected[i]);
        }
    }

    private static void CheckAmplitude(complex actual, complex expected) =>
        Check((actual - expected).Abs() <= Tolerance,
            $"Expected amplitude {expected}, got {actual}");

    private static void CheckClose(double actual, double expected) =>
        Check(Math.Abs(actual - expected) <= Tolerance,
            $"Expected probability {expected}, got {actual}");

    private static void Throws<T>(Action action) where T : Exception
    {
        try
        {
            action();
        }
        catch (T)
        {
            checks++;
            return;
        }
        throw new Exception($"Expected {typeof(T).Name}.");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new Exception(message);
        }
        checks++;
    }

    private sealed class FixedRandom : Random
    {
        private readonly double value;

        public FixedRandom(double value) => this.value = value;

        public override double NextDouble() => value;
    }
}
