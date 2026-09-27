using System;
using Qubit;
using Qubit.Gates;

internal static class Program
{
    private const double Tolerance = 1e-10;
    private static int checks;

    private static void Main()
    {
        CheckQubit(qubit.Zero.Not(), 0, 1);
        CheckQubit(qubit.One.PauliX(), 1, 0);
        CheckQubit(qubit.Zero.Hadamard(), 1 / Math.Sqrt(2), 1 / Math.Sqrt(2));
        CheckQubit(qubit.Zero.Hadamard().Hadamard(), 1, 0);
        CheckQubit(qubit.Zero.Rotate(Math.PI / 2), 0, 1);
        CheckQubit(qubit.Zero.Rotate(Math.PI / 4), 1 / Math.Sqrt(2), 1 / Math.Sqrt(2));
        CheckQubit(qubit.Zero.PauliY(), 0, complex.I);
        CheckQubit(qubit.One.PauliY(), -complex.I, 0);
        CheckQubit(qubit.One.PauliZ(), 0, -1);
        CheckQubit(qubit.Zero.Hadamard().PhaseShift(0, Math.PI / 2),
            1 / Math.Sqrt(2), complex.I / Math.Sqrt(2));

        CheckState(qubit.CNOT(qubit.Zero, qubit.Zero), 1, 0, 0, 0);
        CheckState(qubit.CNOT(qubit.Zero, qubit.One), 0, 1, 0, 0);
        CheckState(qubit.CNOT(qubit.One, qubit.Zero), 0, 0, 0, 1);
        CheckState(qubit.CNOT(qubit.One, qubit.One), 0, 0, 1, 0);

        var bell = qubit.CNOT(qubit.Zero.Hadamard(), qubit.Zero);
        CheckState(bell, 1 / Math.Sqrt(2), 0, 0, 1 / Math.Sqrt(2));
        CheckState(bell.CNOT(), 1 / Math.Sqrt(2), 0, 1 / Math.Sqrt(2), 0);

        var not = new Not { Input = new Pin { Value = qubit.Zero } };
        not.Update();
        CheckQubit(not.Output.Value!, 0, 1);

        var controlledNot = new ControlledNot
        {
            Control = new Pin { Value = qubit.Zero.Hadamard() },
            InputTarget = new Pin { Value = qubit.Zero }
        };
        controlledNot.Update();
        CheckState(controlledNot.OutputState!, 1 / Math.Sqrt(2), 0, 0, 1 / Math.Sqrt(2));

        Throws<ArgumentException>(() => new qubit(0, 0));
        Throws<ArgumentException>(() => new qubit(new complex(double.NaN), 0));
        Throws<ArgumentException>(() => new TwoQubitState(0, 0, 0, 0));
        Throws<InvalidOperationException>(() => new Not().Update());
        Throws<InvalidOperationException>(() => new ControlledNot().Update());

        Check(qubit.Zero == new qubit(1, 0), "Equal qubits");
        Check(qubit.Zero != qubit.One, "Unequal qubits");
        CheckNullComparison();
        Check(qubit.Zero.Equals((object?)null) == false, "Null-safe qubit Equals");
        Check(new complex(0).ToString() == "0", "Complex zero formatting");
        Check(new complex(1).Equals((object?)null) == false, "Null-safe complex Equals");
        Check(qubit.Zero!.GetHashCode() == new qubit(1, 0).GetHashCode(), "Equal qubit hashes");

        Console.WriteLine($"Passed {checks} checks.");
    }

    private static void CheckQubit(qubit actual, complex alpha, complex beta)
    {
        CheckAmplitude(actual.Alpha, alpha);
        CheckAmplitude(actual.Beta, beta);
    }

    private static void CheckNullComparison() =>
        Check(qubit.Zero != null && null != qubit.Zero, "Null-safe qubit comparison");

    private static void CheckState(
        TwoQubitState actual, complex zeroZero, complex zeroOne, complex oneZero, complex oneOne)
    {
        CheckAmplitude(actual.ZeroZero, zeroZero);
        CheckAmplitude(actual.ZeroOne, zeroOne);
        CheckAmplitude(actual.OneZero, oneZero);
        CheckAmplitude(actual.OneOne, oneOne);
    }

    private static void CheckAmplitude(complex actual, complex expected) =>
        Check((actual - expected).Abs() < Tolerance, $"Expected {expected}, got {actual}");

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
}
