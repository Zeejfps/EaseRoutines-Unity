using System;
using System.Collections;
using UnityEngine;

namespace EnvDev
{
    public static class EaseRoutines
    {
        public static IEnumerator Linear(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, x => x);
        }

        public static IEnumerator CubicInOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EasingFunctions.EaseInOutCubic);
        }

        public static IEnumerator CubicIn(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EasingFunctions.EaseInCubic);
        }

        public static IEnumerator CubicOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EasingFunctions.EaseOutCubic);
        }

        public static IEnumerator BounceOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EasingFunctions.EaseOutBounce);
        }

        public static IEnumerator BackOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EasingFunctions.EaseOutBack);
        }

        public static IEnumerator ElasticOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EasingFunctions.EaseOutElastic);
        }

        public static IEnumerator AnimCurve(float duration, Action<float> lerpFunc, AnimationCurve curve)
        {
            return TweenRoutine(duration, lerpFunc, x => curve.Evaluate((float) x));
        }

        public static IEnumerator TweenRoutine(float duration, Action<float> lerpFunc, Func<double, double> easeFunc)
        {
            var time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                var progress = time / duration;
                if (progress > 1f)
                    progress = 1f;
                var t = (float) easeFunc(progress);
                lerpFunc?.Invoke(t);
                yield return null;
            }
        }
    }
}
