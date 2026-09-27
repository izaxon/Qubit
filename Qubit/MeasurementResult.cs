using System;

namespace Qubit
{
    /// <summary>A sampled bit, its probability, and the post-measurement state.</summary>
    public sealed class MeasurementResult
    {
        public int Outcome { get; }
        public double Probability { get; }
        public QuantumRegister State { get; }

        internal MeasurementResult(int outcome, double probability, QuantumRegister state)
        {
            Outcome = outcome;
            Probability = probability;
            State = state ?? throw new ArgumentNullException(nameof(state));
        }
    }
}
