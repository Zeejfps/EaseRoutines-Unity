using System;
using System.Collections;
using System.Collections.Generic;
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
        readonly List<TweenRunner> m_Children = new List<TweenRunner>();

        Coroutine m_Coroutine;

        public TweenRunner(MonoBehaviour target)
        {
            m_Target = target;
        }

        public void Run(IEnumerator tweenRoutine)
        {
            Stop();
            m_Coroutine = m_Target.StartCoroutine(RunRoutine(tweenRoutine));
        }

        public void Run(params IEnumerator[] tweenRoutines)
        {
            if (tweenRoutines.Length == 1)
            {
                Run(tweenRoutines[0]);
                return;
            }

            Stop();

            while (tweenRoutines.Length > m_Children.Count)
                m_Children.Add(new TweenRunner(m_Target));

            for (var i = 0; i < tweenRoutines.Length; i++)
                m_Children[i].Run(tweenRoutines[i]);

            m_Coroutine = m_Target.StartCoroutine(WaitForAll(m_Children));
        }

        /// <summary>
        /// Stops all tweens
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
                return;

            if (m_Coroutine != null)
                m_Target.StopCoroutine(m_Coroutine);

            foreach (var child in m_Children)
                child.Stop();

            IsRunning = false;
            Stopped?.Invoke();
            Interrupted?.Invoke();
        }

        IEnumerator RunRoutine(IEnumerator routine)
        {
            IsRunning = true;
            Started?.Invoke();

            yield return routine;

            IsRunning = false;
            Stopped?.Invoke();
            Completed?.Invoke();
        }

        IEnumerator WaitForAll(List<TweenRunner> runners)
        {
            IsRunning = true;
            Started?.Invoke();

            while (runners.Exists(r => r.IsRunning))
                yield return null;

            IsRunning = false;
            Stopped?.Invoke();
            Completed?.Invoke();
        }
    }

}