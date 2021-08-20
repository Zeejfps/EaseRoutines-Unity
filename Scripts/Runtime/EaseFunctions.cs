using System;


namespace EnvDev
{
    /// <summary>
    /// Ease functions based of this cheat sheet: https://easings.net/
    /// </summary>
    public static class EasingFunctions
    {
        const double N1 = 7.5635;
        const double D1 = 2.75;
        const double C1 = 1.70158;
        const double C3 = C1 + 1;
        const double C4 = (2.0 * Math.PI) / 3.0;

        #region Cubic

        public static double EaseOutCubic(double x)
        {
            return 1.0 - Math.Pow(1.0 - x, 3.0);
        }

        public static double EaseInCubic(double x)
        {
            return x * x * x;
        }

        public static double EaseInOutCubic(double x)
        {
            return x < 0.5 ? 4.0 * x * x * x : 1.0 - Math.Pow(-2.0 * x + 2.0, 3.0) / 2.0;
        }

        #endregion

        #region Bounce

        public static double EaseOutBounce(double x)
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

        #endregion

        #region Back

        public static double EaseOutBack(double x)
        {
            return 1 + C3 * Math.Pow(x - 1, 3) + C1 * Math.Pow(x - 1, 2);
        }

        #endregion

        #region Elastic

        public static double EaseOutElastic(double x)
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