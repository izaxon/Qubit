using System;

namespace Qubit
{
    /// <summary>
    /// An immutable state vector for a small register. Qubit 0 is the
    /// leftmost (most significant) bit in each computational basis label.
    /// </summary>
    public sealed class QuantumRegister : IEquatable<QuantumRegister>
    {
        public const int MaxQubits = 16;
        private const double NormalizationTolerance = 1e-10;
        private readonly complex[] amplitudes;

        public int QubitCount { get; }
        public int BasisStateCount => amplitudes.Length;

        public QuantumRegister(int qubitCount, params complex[] amplitudes)
        {
            if (qubitCount < 1 || qubitCount > MaxQubits)
            {
                throw new ArgumentOutOfRangeException(nameof(qubitCount),
                    $"A register must contain between 1 and {MaxQubits} qubits.");
            }
            ArgumentNullException.ThrowIfNull(amplitudes);
            if (amplitudes.Length != 1 << qubitCount)
            {
                throw new ArgumentException("The amplitude count must equal 2^qubitCount.",
                    nameof(amplitudes));
            }

            double norm = 0;
            foreach (complex amplitude in amplitudes)
            {
                norm += SquaredMagnitude(amplitude);
            }
            if (!double.IsFinite(norm) || Math.Abs(norm - 1) > NormalizationTolerance)
            {
                throw new ArgumentException(
                    "Register amplitudes must have a finite squared norm of one.",
                    nameof(amplitudes));
            }

            QubitCount = qubitCount;
            this.amplitudes = (complex[])amplitudes.Clone();
        }

        public static QuantumRegister Zero(int qubitCount)
        {
            ValidateQubitCount(qubitCount);
            var values = new complex[1 << qubitCount];
            values[0] = complex.One;
            return new QuantumRegister(qubitCount, values);
        }

        public static QuantumRegister FromProduct(params qubit[] qubits)
        {
            ArgumentNullException.ThrowIfNull(qubits);
            ValidateQubitCount(qubits.Length);
            complex[] values = { complex.One };
            foreach (qubit? q in qubits)
            {
                ArgumentNullException.ThrowIfNull(q);
                var next = new complex[values.Length * 2];
                for (int i = 0; i < values.Length; i++)
                {
                    next[2 * i] = values[i] * q.Alpha;
                    next[2 * i + 1] = values[i] * q.Beta;
                }
                values = next;
            }
            return new QuantumRegister(qubits.Length, values);
        }

        public complex Amplitude(int basisIndex)
        {
            if ((uint)basisIndex >= (uint)amplitudes.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(basisIndex));
            }
            return amplitudes[basisIndex];
        }

        public complex[] ToArray() => (complex[])amplitudes.Clone();

        public QuantumRegister ApplyX(int target) =>
            ApplySingle(target, 0, 1, 1, 0);

        public QuantumRegister ApplyY(int target) =>
            ApplySingle(target, 0, -complex.I, complex.I, 0);

        public QuantumRegister ApplyZ(int target) =>
            ApplySingle(target, 1, 0, 0, -1);

        public QuantumRegister ApplyHadamard(int target)
        {
            double scale = 1 / Math.Sqrt(2);
            return ApplySingle(target, scale, scale, scale, -scale);
        }

        /// <summary>Applies [cos(angle) -sin(angle); sin(angle) cos(angle)].</summary>
        public QuantumRegister ApplyRotation(int target, double radians)
        {
            double cosine = Math.Cos(radians);
            double sine = Math.Sin(radians);
            return ApplySingle(target, cosine, -sine, sine, cosine);
        }

        public QuantumRegister ApplyPhaseShift(int target, double theta, double phi) =>
            ApplySingle(target, complex.Exp(theta), 0, 0, complex.Exp(phi));

        /// <summary>Flips target when control is |1>, preserving entanglement.</summary>
        public QuantumRegister ApplyCNOT(int control, int target)
        {
            int controlMask = MaskFor(control);
            int targetMask = MaskFor(target);
            if (control == target)
            {
                throw new ArgumentException("Control and target must be different qubits.");
            }
            var result = new complex[amplitudes.Length];
            for (int basis = 0; basis < amplitudes.Length; basis++)
            {
                int destination = (basis & controlMask) == 0 ? basis : basis ^ targetMask;
                result[destination] = amplitudes[basis];
            }
            return new QuantumRegister(QubitCount, result);
        }

        /// <summary>Probability of measuring outcome 0 or 1 on one qubit.</summary>
        public double ProbabilityOf(int target, int outcome)
        {
            int mask = MaskFor(target);
            if (outcome != 0 && outcome != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(outcome));
            }
            double selected = 0;
            double total = 0;
            for (int basis = 0; basis < amplitudes.Length; basis++)
            {
                double probability = SquaredMagnitude(amplitudes[basis]);
                total += probability;
                if (((basis & mask) == 0 ? 0 : 1) == outcome)
                {
                    selected += probability;
                }
            }
            return selected / total;
        }

        /// <summary>
        /// Samples a computational-basis measurement and returns the outcome,
        /// its probability, and the collapsed register. Pass a Random instance
        /// for reproducible sampling.
        /// </summary>
        public MeasurementResult Measure(int target, Random? random = null)
        {
            int mask = MaskFor(target);
            double probabilityZero = ProbabilityOf(target, 0);
            int outcome = (random ?? Random.Shared).NextDouble() < probabilityZero ? 0 : 1;
            double probability = outcome == 0 ? probabilityZero : 1 - probabilityZero;

            var collapsed = new complex[amplitudes.Length];
            double selectedNorm = 0;
            for (int basis = 0; basis < amplitudes.Length; basis++)
            {
                if (((basis & mask) == 0 ? 0 : 1) == outcome)
                {
                    selectedNorm += SquaredMagnitude(amplitudes[basis]);
                }
            }
            double scale = 1 / Math.Sqrt(selectedNorm);
            for (int basis = 0; basis < amplitudes.Length; basis++)
            {
                if (((basis & mask) == 0 ? 0 : 1) == outcome)
                {
                    collapsed[basis] = amplitudes[basis] * scale;
                }
            }
            return new MeasurementResult(outcome, probability,
                new QuantumRegister(QubitCount, collapsed));
        }

        /// <summary>
        /// Compares physical pure states within a tolerance, ignoring a
        /// common global phase. Equals still compares amplitudes exactly.
        /// </summary>
        public bool PhysicallyEquivalentTo(QuantumRegister? other, double tolerance = 1e-10)
        {
            if (!double.IsFinite(tolerance) || tolerance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }
            if (other is null || QubitCount != other.QubitCount)
            {
                return false;
            }

            int pivot = 0;
            double largest = 0;
            for (int i = 0; i < amplitudes.Length; i++)
            {
                double magnitude = SquaredMagnitude(other.amplitudes[i]);
                if (magnitude > largest)
                {
                    largest = magnitude;
                    pivot = i;
                }
            }

            if (amplitudes[pivot] == complex.Zero)
            {
                return false;
            }
            complex phase = amplitudes[pivot] / other.amplitudes[pivot];
            phase = phase / phase.Abs();
            for (int i = 0; i < amplitudes.Length; i++)
            {
                if ((amplitudes[i] - phase * other.amplitudes[i]).Abs() > tolerance)
                {
                    return false;
                }
            }
            return true;
        }

        private QuantumRegister ApplySingle(
            int target, complex m00, complex m01, complex m10, complex m11)
        {
            int mask = MaskFor(target);
            var result = new complex[amplitudes.Length];
            for (int basis = 0; basis < amplitudes.Length; basis++)
            {
                if ((basis & mask) != 0)
                {
                    continue;
                }
                complex zero = amplitudes[basis];
                complex one = amplitudes[basis | mask];
                result[basis] = m00 * zero + m01 * one;
                result[basis | mask] = m10 * zero + m11 * one;
            }
            return new QuantumRegister(QubitCount, result);
        }

        private int MaskFor(int qubitIndex)
        {
            if ((uint)qubitIndex >= (uint)QubitCount)
            {
                throw new ArgumentOutOfRangeException(nameof(qubitIndex));
            }
            return 1 << (QubitCount - 1 - qubitIndex);
        }

        private static void ValidateQubitCount(int count)
        {
            if (count < 1 || count > MaxQubits)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
        }

        private static double SquaredMagnitude(complex value) =>
            value.Real * value.Real + value.Imaginary * value.Imaginary;

        public bool Equals(QuantumRegister? other)
        {
            if (other is null || QubitCount != other.QubitCount)
            {
                return false;
            }
            for (int i = 0; i < amplitudes.Length; i++)
            {
                if (amplitudes[i] != other.amplitudes[i])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object? obj) =>
            obj is QuantumRegister other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(QubitCount);
            foreach (complex amplitude in amplitudes)
            {
                hash.Add(amplitude);
            }
            return hash.ToHashCode();
        }
    }
}
