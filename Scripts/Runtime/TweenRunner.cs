using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace EnvDev
{
    public class TweenRunner
    {
        public event Action Started;
        public event Action Stopped;
        public event Action Completed;
        public event Action Interrupted;

        /// <summary>
        /// True if the runner has any tweens playing
        /// </summary>
        public bool IsRunning { get; private set; }

        readonly MonoBehaviour m_Target;
        readonly List<TweenRunner> m_Runners = new List<TweenRunner>();

        Coroutine m_Coroutine;

        public TweenRunner(MonoBehaviour target)
        {
            m_Target = target;
        }

        public void Run(IEnumerator tweenRoutine)
        {
            Interrupt();
            m_Coroutine = m_Target.StartCoroutine(RunRoutine(tweenRoutine));
        }

        public void Run(params IEnumerator[] tweenRoutines)
        {
            if (tweenRoutines.Length == 1)
            {
                Run(tweenRoutines[0]);
                return;
            }

            Interrupt();

            var tweenCount = tweenRoutines.Length;
            
            while (tweenCount > m_Runners.Count)
                m_Runners.Add(new TweenRunner(m_Target));

            for (var i = 0; i < tweenCount; i++)
                m_Runners[i].Run(tweenRoutines[i]);

            m_Coroutine = m_Target.StartCoroutine(WaitForAll(m_Runners, tweenCount));
        }

        /// <summary>
        /// Stops all tweens
        /// </summary>
        public void Interrupt()
        {
            if (!IsRunning)
                return;

            if (m_Coroutine != null)
                m_Target.StopCoroutine(m_Coroutine);

            foreach (var child in m_Runners)
                child.Interrupt();

            IsRunning = false;
            Interrupted?.Invoke();
            Stopped?.Invoke();
        }

        IEnumerator RunRoutine(IEnumerator routine)
        {
            IsRunning = true;
            Started?.Invoke();

            yield return routine;

            IsRunning = false;
            Completed?.Invoke();
            Stopped?.Invoke();
        }

        IEnumerator WaitForAll(List<TweenRunner> runners, int tweenCount)
        {
            IsRunning = true;
            Started?.Invoke();

            var runningTweenIndex = 0;
            while (true)
            {
                if (runners[runningTweenIndex].IsRunning)
                {
                    yield return null;
                }
                else
                {
                    runningTweenIndex++;
                    if (runningTweenIndex >= tweenCount)
                        break;
                }
            }

            IsRunning = false;
            Completed?.Invoke();
            Stopped?.Invoke();
        }
    }

}