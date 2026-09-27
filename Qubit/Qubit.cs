using System;

namespace Qubit
{
    /// <summary>A normalized, immutable single-qubit state.</summary>
    public sealed class qubit : IEquatable<qubit>
    {
        private const double NormalizationTolerance = 1e-10;

        public complex Alpha { get; }
        public complex Beta { get; }

        public qubit(complex alpha, complex beta)
        {
            double norm = alpha.Real * alpha.Real + alpha.Imaginary * alpha.Imaginary
                + beta.Real * beta.Real + beta.Imaginary * beta.Imaginary;
            if (!double.IsFinite(norm) || Math.Abs(norm - 1) > NormalizationTolerance)
            {
                throw new ArgumentException("Qubit amplitudes must have a finite squared norm of one.");
            }

            Alpha = alpha;
            Beta = beta;
        }

        public static readonly qubit Zero = new qubit(complex.One, complex.Zero);
        public static readonly qubit One = new qubit(complex.Zero, complex.One);

        public qubit Not() => new qubit(Beta, Alpha);

        public qubit Hadamard() =>
            new qubit((Alpha + Beta) / Math.Sqrt(2), (Alpha - Beta) / Math.Sqrt(2));

        /// <summary>Applies the real rotation matrix [cos -sin; sin cos].</summary>
        public qubit Rotate(double radians)
        {
            double cosine = Math.Cos(radians);
            double sine = Math.Sin(radians);
            return new qubit(cosine * Alpha - sine * Beta, sine * Alpha + cosine * Beta);
        }

        public qubit PauliX() => Not();

        public qubit PauliY() => new qubit(-complex.I * Beta, complex.I * Alpha);

        public qubit PauliZ() => new qubit(Alpha, -Beta);

        public qubit PhaseShift(double theta, double phi) =>
            new qubit(complex.Exp(theta) * Alpha, complex.Exp(phi) * Beta);

        /// <summary>
        /// Applies a controlled NOT to a product input and returns the complete
        /// two-qubit state, which may be entangled.
        /// </summary>
        public static TwoQubitState CNOT(qubit control, qubit target)
        {
            return TwoQubitState.FromProduct(control, target).CNOT();
        }

        public bool Equals(qubit? other) =>
            other is not null && Alpha == other.Alpha && Beta == other.Beta;

        public override bool Equals(object? obj) => obj is qubit other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Alpha, Beta);

        public static bool operator ==(qubit? left, qubit? right) =>
            ReferenceEquals(left, right) || (left is not null && left.Equals(right));

        public static bool operator !=(qubit? left, qubit? right) => !(left == right);

        public override string ToString() => $"{Alpha}|0> + {Beta}|1>";
    }
}
