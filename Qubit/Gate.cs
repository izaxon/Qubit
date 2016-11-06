using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qubit
{
    public class Gate
    {
        public List<Pin> Inputs = new List<Pin>();
        public List<Pin> Outputs = new List<Pin>();
        
        public virtual void Update()
        {
        }
    }
}
