using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qubit.Gates
{
    public class ControlledNot : Gate
    {
        public Pin Control = new Pin();
        public Pin InputTarget = new Pin();
        public TwoQubitState? OutputState { get; private set; }

        public override void Update()
        {
            OutputState = qubit.CNOT(
                Control.Value ?? throw new InvalidOperationException("Control pin has no qubit."),
                InputTarget.Value ?? throw new InvalidOperationException("Target pin has no qubit."));
        }
    }
}
