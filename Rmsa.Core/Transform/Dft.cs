using System;
using System.Numerics;

namespace Rmsa.Transform
{
    public class Dft
    {
        /// <summary>
        /// Вычисление поворачивающего модуля e^(-i*2*PI*k*n/N)
        /// </summary>
        /// <param name="k"></param>
        /// <param name="N"></param>
        /// <returns></returns>
        private static Complex W(int k, int n, int N)
        {
            //if (k % N == 0) return 1;
            double arg = 2 * Math.PI * k * n / N;
            return new Complex(Math.Cos(arg), -Math.Sin(arg));
        }

        internal static Complex[] Transform(Complex[] data)
        {
            int N = data.Length;
            var result = new Complex[N];

            for (int k = 0; k < N; k++)
            {
                Complex Xk = new Complex(0, 0);

                for (int n = 0; n < N; n++)
                {
                    double Xn = data[n].Real;
                    var Xkn = Xn * W(k, n, N);
                    Xk += Xkn;
                }

                result[k] = Xk;
            }

            return result;
        }

    }
}
