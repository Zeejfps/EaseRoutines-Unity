using System;
using System.Runtime.CompilerServices;


namespace EnvDev
{
    /// <summary>
    /// Ease functions based of this cheat sheet: https://easings.net/
    /// </summary>
    public static class EaseFunctions
    {
        const double N1 = 7.5635;
        const double D1 = 2.75;
        const double C1 = 1.70158;
        const double C2 = C1 * 1.525;
        const double C3 = C1 + 1;
        const double C4 = (2.0 * Math.PI) / 3.0;

        #region Sine

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SineIn(double x)
        {
            return 1.0 - Math.Cos((x * Math.PI) / 2.0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SineOut(double x)
        {
            return Math.Sin((x * Math.PI) / 2.0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double SineInOut(double x)
        {
            return -(Math.Cos(Math.PI * x) - 1.0) / 2.0;
        }

        #endregion
        
        #region Cubic

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CubicOut(double x)
        {
            return 1.0 - Math.Pow(1.0 - x, 3.0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CubicIn(double x)
        {
            return x * x * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CubicInOut(double x)
        {
            return x < 0.5 ? 4.0 * x * x * x : 1.0 - Math.Pow(-2.0 * x + 2.0, 3.0) / 2.0;
        }

        #endregion

        #region Quad

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double QuadIn(double x)
        {
            return x * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double QuadOut(double x)
        {
            return 1.0 - (1.0 - x) * (1.0 - x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double QuadInOut(double x)
        {
            return x < 0.5 ? 2.0 * x * x : 1.0 - Math.Pow(-2.0 * x + 2.0, 2.0) / 2.0;
        }
        
        #endregion

        #region Bounce

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BounceIn(double x)
        {
            return 1.0 - BounceOut(1.0 - x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BounceOut(double x)
        {
            if (x < 1 / D1)
            {
                return N1 * x * x;
            }

            if (x < 2 / D1)
            {
                return N1 * (x -= 1.5 / D1) * x + 0.75;
            }

            if (x < 2.5 / D1)
            {
                return N1 * (x -= 2.25 / D1) * x + 0.9375;
            }

            return N1 * (x -= 2.625 / D1) * x + 0.984375;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BounceInOut(double x)
        {
            return x < 0.5
                ? (1.0 - BounceOut(1.0 - 2.0 * x)) / 2.0
                : (1.0 + BounceOut(2.0 * x - 1.0)) / 2.0;
        }

        #endregion

        #region Back

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BackIn(double x)
        {
            return C3 * x * x * x - C1 * x * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BackOut(double x)
        {
            return 1 + C3 * Math.Pow(x - 1, 3) + C1 * Math.Pow(x - 1, 2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double BackInOut(double x)
        {
            return x < 0.5
                ? (Math.Pow(2.0 * x, 2.0) * ((C2 + 1) * 2 * x - C2)) / 2
                : (Math.Pow(2.0 * x - 2.0, 2.0) * ((C2 + 1) * (x * 2 - 2) + C2) + 2) / 2.0;
        }

        #endregion

        #region Elastic
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ElasticIn(double x)
        {
            return x == 0
                ? 0
                : Math.Abs(x - 1.0) < 0.000000001
                    ? 1.0
                    : -Math.Pow(2, 10 * x - 10) * Math.Sin((x * 10 - 10.75) * C4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ElasticOut(double x)
        {
            if (x == 0.0)
                return 0.0;

            if (Math.Abs(x - 1.0) < double.Epsilon)
                return 1.0;

            return Math.Pow(2.0, -10.0 * x) * Math.Sin((x * 10.0 - 0.75) * C4) + 1.0;
        }

        #endregion
    }

}