using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qubit.Gates
{
    public class ControlledNot : Gate
    {
        public Pin Control;
        public Pin InputTarget;
        public Pin OutputTarget;

        public override void Update()
        {
            OutputTarget.Value = qubit.CNOT(Control.Value, InputTarget.Value);
        }
    }
}
