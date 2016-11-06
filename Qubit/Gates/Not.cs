using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qubit.Gates
{
    public class Not : Gate
    {
        public Pin Input;
        public Pin Output;

        public override void Update()
        {
            Output.Value = Input.Value.Not();
        }
    }
}
