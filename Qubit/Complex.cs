using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qubit
{
    public struct complex
    {
        public double Real;
        public double Imaginary;

        public complex(double real, double imaginary)
        {
            this.Real = real;
            this.Imaginary = imaginary;
        }

        public complex(complex c)
        {
            this.Real = c.Real;
            this.Imaginary = c.Imaginary;
        }

        public complex(double real)
        {
            this.Real = real;
            this.Imaginary = 0;
        }

        public static readonly complex I = new complex(0, 1);
        public static readonly complex Zero = new complex(0, 0);
        public static readonly complex One = new complex(1, 0);

        public static complex operator -(complex c)
        {
            return new complex(-c.Real, -c.Imaginary);
        }

        public static complex operator +(complex c1, complex c2)
        {
            return new complex(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
        }

        public static complex operator -(complex c1, complex c2)
        {
            return new complex(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
        }

        public static implicit operator complex(double real)
        {
            return new complex(real);
        }

        public static complex operator *(complex c1, complex c2)
        {
            return new complex(c1.Real * c2.Real - c1.Imaginary * c2.Imaginary, c1.Real * c2.Imaginary + c1.Imaginary * c2.Real);
        }

        public static complex operator /(complex c, double divisor)
        {
            return new complex(c.Real / divisor, c.Imaginary / divisor);
        }

        public static complex operator /(complex c1, complex c2)
        {
            return c1 * c2.Conjugate() / (c2.Real * c2.Real + c2.Imaginary * c2.Imaginary);
        }

        public double Abs()
        {
            return Math.Sqrt(Math.Pow(Real, 2) + Math.Pow(Imaginary, 2));
        }

        public complex Conjugate()
        {
            return new complex(Real, -Imaginary);
        }

        /// <summary>
        /// Create complex from Euler's formula.
        /// </summary>
        /// <param name="radians">Radians in radians</param>
        /// <returns>cos(x) + i*sin(x)</returns>
        public static complex Exp(double radians)
        {
            return new complex(Math.Cos(radians), Math.Sin(radians));
        }

        public static bool operator ==(complex c1, complex c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(complex c1, complex c2)
        {
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            complex c = (complex)obj;
            return c.Real == Real && c.Imaginary == Imaginary;
        }

        // Override the ToString() method to display a complex number 
        // in the traditional format:
        public override string ToString()
        {
            string temp = "";
            if (Real != 0)
            {
                temp += string.Format("{0}", Real);
            }
            if (Imaginary != 0)
            {
                if (Imaginary < 0)
                {
                    temp += string.Format("-{0}i", -Imaginary);
                }
                else
                {
                    if (temp != "")
                    {
                        temp += string.Format("+{0}i", Imaginary);
                    }
                    else
                    {
                        temp += string.Format("{0}i", Imaginary);
                    }
                }
            }
            return temp;
        }
    }
}
