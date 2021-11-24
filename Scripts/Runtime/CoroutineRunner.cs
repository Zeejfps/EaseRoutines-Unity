using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace EnvDev
{
    public class CoroutineRunner
    {
        public event Action Started;
        public event Action Stopped;
        public event Action Completed;
        public event Action Interrupted;

        /// <summary>
        /// True if the runner has any coroutines that are running
        /// </summary>
        public bool IsRunning { get; private set; }

        readonly MonoBehaviour m_Target;
        readonly List<CoroutineRunner> m_Runners = new List<CoroutineRunner>();

        Coroutine m_Coroutine;

        public CoroutineRunner(MonoBehaviour target)
        {
            m_Target = target;
        }

        public void Run(IEnumerator tweenRoutine)
        {
            Interrupt();
            m_Coroutine = m_Target.StartCoroutine(WaitFor(tweenRoutine));
        }

        public void Run(params IEnumerator[] coroutines)
        {
            var coroutineCount = coroutines.Length;

            if (coroutineCount == 1)
            {
                Run(coroutines[0]);
                return;
            }

            Interrupt();

            while (coroutineCount > m_Runners.Count)
                m_Runners.Add(new CoroutineRunner(m_Target));

            for (var i = 0; i < coroutineCount; i++)
                m_Runners[i].Run(coroutines[i]);

            m_Coroutine = m_Target.StartCoroutine(WaitForAll(m_Runners, coroutineCount));
        }

        /// <summary>
        /// Stops execution of all coroutines
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

        IEnumerator WaitFor(IEnumerator coroutine)
        {
            IsRunning = true;
            Started?.Invoke();

            yield return coroutine;

            IsRunning = false;
            Completed?.Invoke();
            Stopped?.Invoke();
        }

        IEnumerator WaitForAll(List<CoroutineRunner> runners, int coroutineCount)
        {
            IsRunning = true;
            Started?.Invoke();

            var runningCoroutineIndex = 0;
            while (true)
            {
                if (runners[runningCoroutineIndex].IsRunning)
                {
                    yield return null;
                }
                else
                {
                    runningCoroutineIndex++;
                    if (runningCoroutineIndex >= coroutineCount)
                        break;
                }
            }

            IsRunning = false;
            Completed?.Invoke();
            Stopped?.Invoke();
        }
    }

}