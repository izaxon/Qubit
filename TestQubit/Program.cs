using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qubit;

namespace TestQubit
{
    class Program
    {
        static void Main(string[] args)
        {
            qubit q0 = qubit.Zero;
            qubit q1 = qubit.One;
            Console.WriteLine(q0);
            Console.WriteLine(q1);
            Console.WriteLine(qubit.CNOT(q0, q0));
            Console.WriteLine(qubit.CNOT(q0, q1));
            Console.WriteLine(qubit.CNOT(q1, q0));
            Console.WriteLine(qubit.CNOT(q1, q1));

            Tests.Test1();

            // Test with Hadarmard on q0 as control...
            qubit control = qubit.Zero.Hadamard();
            Console.WriteLine(qubit.CNOT(control, q0));
            Console.WriteLine(qubit.CNOT(control, q1));
            control = qubit.One.Hadamard();
            Console.WriteLine(qubit.CNOT(control, q0));
            Console.WriteLine(qubit.CNOT(control, q1));

            qubit target = qubit.One.Hadamard();
            Console.WriteLine(qubit.CNOT(q0, target));
            Console.WriteLine(qubit.CNOT(q1, target));
        }
    }
}
