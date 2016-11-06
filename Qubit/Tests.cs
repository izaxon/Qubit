using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Qubit
{
    public class Tests
    {
        /// <summary>
        /// CNOT gate tests.
        /// </summary>
        public static void Test1()
        {
            qubit q0 = qubit.Zero;
            qubit q1 = qubit.One;
            Debug.Assert(qubit.CNOT(q0, q0) == q0);
            Debug.Assert(qubit.CNOT(q0, q1) == q1);
            Debug.Assert(qubit.CNOT(q1, q0) == q1);
            Debug.Assert(qubit.CNOT(q1, q1) == q0);
        }
    }
}
