using System;

namespace Qubit
{
    /// <summary>Amplitudes in the order |00>, |01>, |10>, |11>.</summary>
    public sealed class TwoQubitState : IEquatable<TwoQubitState>
    {
        private const double NormalizationTolerance = 1e-10;

        public complex ZeroZero { get; }
        public complex ZeroOne { get; }
        public complex OneZero { get; }
        public complex OneOne { get; }

        public TwoQubitState(complex zeroZero, complex zeroOne, complex oneZero, complex oneOne)
        {
            double norm = SquaredMagnitude(zeroZero) + SquaredMagnitude(zeroOne)
                + SquaredMagnitude(oneZero) + SquaredMagnitude(oneOne);
            if (!double.IsFinite(norm) || Math.Abs(norm - 1) > NormalizationTolerance)
            {
                throw new ArgumentException("Two-qubit amplitudes must have a finite squared norm of one.");
            }

            ZeroZero = zeroZero;
            ZeroOne = zeroOne;
            OneZero = oneZero;
            OneOne = oneOne;
        }

        public static TwoQubitState FromProduct(qubit first, qubit second)
        {
            ArgumentNullException.ThrowIfNull(first);
            ArgumentNullException.ThrowIfNull(second);
            return new TwoQubitState(
                first.Alpha * second.Alpha,
                first.Alpha * second.Beta,
                first.Beta * second.Alpha,
                first.Beta * second.Beta);
        }

        /// <summary>Uses the first qubit as control and the second as target.</summary>
        public TwoQubitState CNOT() =>
            new TwoQubitState(ZeroZero, ZeroOne, OneOne, OneZero);

        private static double SquaredMagnitude(complex value) =>
            value.Real * value.Real + value.Imaginary * value.Imaginary;

        public bool Equals(TwoQubitState? other) =>
            other is not null
            && ZeroZero == other.ZeroZero
            && ZeroOne == other.ZeroOne
            && OneZero == other.OneZero
            && OneOne == other.OneOne;

        public override bool Equals(object? obj) =>
            obj is TwoQubitState other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(ZeroZero, ZeroOne, OneZero, OneOne);

        public override string ToString() =>
            $"{ZeroZero}|00> + {ZeroOne}|01> + {OneZero}|10> + {OneOne}|11>";
    }
}
