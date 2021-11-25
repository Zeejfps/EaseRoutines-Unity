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
            return TweenRoutine(duration, lerpFunc, EaseFunctions.CubicInOut);
        }

        public static IEnumerator CubicIn(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EaseFunctions.CubicIn);
        }

        public static IEnumerator CubicOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EaseFunctions.CubicOut);
        }

        public static IEnumerator BounceOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EaseFunctions.BounceOut);
        }

        public static IEnumerator BackOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EaseFunctions.BackOut);
        }

        public static IEnumerator ElasticOut(float duration, Action<float> lerpFunc)
        {
            return TweenRoutine(duration, lerpFunc, EaseFunctions.ElasticOut);
        }

        public static IEnumerator AnimCurve(float duration, Action<float> lerpFunc, AnimationCurve curve)
        {
            return TweenRoutine(duration, lerpFunc, x => curve.Evaluate((float) x));
        }

        public static IEnumerator TweenRoutine(float duration, Action<float> lerpFunc, Func<double, double> easeFunc)
        {
            var time = Time.deltaTime;
            while (time < duration)
            {
                var progress = time / duration;
                lerpFunc.Invoke((float) easeFunc(progress));
                yield return null;
                time += Time.deltaTime;
            }
            lerpFunc.Invoke((float) easeFunc(1f));
        }
    }
}
