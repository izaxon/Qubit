using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qubit.Gates
{
    public class Not : Gate
    {
        public Pin Input = new Pin();
        public Pin Output = new Pin();

        public override void Update()
        {
            Output.Value = (Input.Value ?? throw new InvalidOperationException("Input pin has no qubit.")).Not();
        }
    }
}
