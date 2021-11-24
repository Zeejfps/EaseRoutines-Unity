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
        int m_ActiveCoroutineCount;
        int m_RunningCoroutineIndex;

        public CoroutineRunner(MonoBehaviour target)
        {
            m_Target = target;
        }

        public void Run(params IEnumerator[] coroutines)
        {
            while (m_ActiveCoroutineCount + coroutines.Length > m_Runners.Count)
                m_Runners.Add(new CoroutineRunner(m_Target));

            for (var i = 0; i < coroutines.Length; i++)
                m_Runners[m_ActiveCoroutineCount + i].RunSingle(coroutines[i]);

            m_ActiveCoroutineCount += coroutines.Length;

            if (!IsRunning)
                RunAll();
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

            for (var i = m_RunningCoroutineIndex; i < m_ActiveCoroutineCount; i++)
                m_Runners[i].Interrupt();

            IsRunning = false;
            OnInterrupted();
            OnStopped();
        }

        void RunAll()
        {
            m_Coroutine = m_Target.StartCoroutine(WaitForAll());
        }

        void RunSingle(IEnumerator coroutine)
        {
            m_Coroutine = m_Target.StartCoroutine(WaitFor(coroutine));
        }

        void OnStarted()
        {
            Started?.Invoke();
        }

        void OnInterrupted()
        {
            Interrupted?.Invoke();
        }
        
        void OnCompleted()
        {
            Completed?.Invoke();
        }
        
        void OnStopped()
        {
            m_ActiveCoroutineCount = 0;
            m_RunningCoroutineIndex = 0;
            Stopped?.Invoke();
        }

        IEnumerator WaitFor(IEnumerator coroutine)
        {
            IsRunning = true;
            OnStarted();
            
            yield return coroutine;

            IsRunning = false;
            OnCompleted();
            OnStopped();
        }
        
        IEnumerator WaitForAll()
        {
            IsRunning = true;
            OnStarted();

            while (true)
            {
                if (m_Runners[m_RunningCoroutineIndex].IsRunning)
                {
                    yield return null;
                }
                else
                {
                    m_RunningCoroutineIndex++;
                    if (m_RunningCoroutineIndex >= m_ActiveCoroutineCount)
                        break;
                }
            }

            IsRunning = false;
            OnCompleted();
            OnStopped();
        }
    }

}