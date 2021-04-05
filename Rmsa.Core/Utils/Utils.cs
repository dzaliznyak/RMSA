using DSPLib;
using System.Numerics;

namespace Rmsa
{
    public static class Utils
    {
        public static Complex[] RToComplex(double[] r)
        {
            var complex = new Complex[r.Length];

            for (int i = 0; i < r.Length; i++)
            {
                complex[i] = new Complex(r[i], 0);
            }

            return complex;
        }

        internal static double[] ComplexToResultType(Complex[] cpxResult, ResultType resultType, double windowScaleFactor)
        {
            double[] res = null;

            if (resultType == ResultType.LogMagnitude)
            {
                // log magnitude
                double[] mag = DSP.ConvertComplex.ToMagnitude(cpxResult);
                mag = DSP.Math.Multiply(mag, windowScaleFactor);
                res = DSP.ConvertMagnitude.ToMagnitudeDBV(mag);
            }
            else if (resultType == ResultType.Magnitude)
            {
                // Convert the complex result to a scalar magnitude 
                res = DSP.ConvertComplex.ToMagnitude(cpxResult);
                res = DSP.Math.Multiply(res, windowScaleFactor);
            }
            else if (resultType == ResultType.Real)
            {
                res = DSP.ConvertComplex.ToReal(cpxResult);
            }
            else if (resultType == ResultType.Imaginary)
            {
                res = DSP.ConvertComplex.ToImaginary(cpxResult);
            }
            else if (resultType == ResultType.Phase)
            {
                res = DSP.ConvertComplex.ToPhase(cpxResult);
            }
            else if (resultType == ResultType.MagnitudeSquared)
            {
                res = DSP.ConvertComplex.ToMagnitudeSquared(cpxResult);
            }
            else if (resultType == ResultType.PhaseDegrees)
            {
                res = DSP.ConvertComplex.ToPhaseDegrees(cpxResult);
            }
            else if (resultType == ResultType.PhaseRadians)
            {
                res = DSP.ConvertComplex.ToPhaseRadians(cpxResult);
            }

            return res;
        }
    }
}
