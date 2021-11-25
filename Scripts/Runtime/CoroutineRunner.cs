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
        readonly List<CoroutineRunner> m_RunnersPool = new List<CoroutineRunner>();

        Coroutine m_Coroutine;
        int m_ActiveRunnerCount;
        int m_RunningCoroutineIndex;

        public CoroutineRunner(MonoBehaviour target)
        {
            m_Target = target;
        }

        public void Run(params IEnumerator[] coroutines)
        {
            var coroutineCount = coroutines.Length;
            var newActiveRunnerCount = m_ActiveRunnerCount + coroutineCount;
            
            while (newActiveRunnerCount > m_RunnersPool.Count)
                m_RunnersPool.Add(new CoroutineRunner(m_Target));

            for (var i = 0; i < coroutineCount; i++)
                m_RunnersPool[m_ActiveRunnerCount + i].RunSingle(coroutines[i]);

            m_ActiveRunnerCount = newActiveRunnerCount;

            if (!IsRunning) StartRunning();
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

            for (var i = m_RunningCoroutineIndex; i < m_ActiveRunnerCount; i++)
                m_RunnersPool[i].Interrupt();

            IsRunning = false;
            OnInterrupted();
            OnStopped();
        }

        void StartRunning()
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
            m_ActiveRunnerCount = 0;
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
                if (m_RunnersPool[m_RunningCoroutineIndex].IsRunning)
                {
                    yield return null;
                }
                else
                {
                    m_RunningCoroutineIndex++;
                    if (m_RunningCoroutineIndex >= m_ActiveRunnerCount)
                        break;
                }
            }

            IsRunning = false;
            OnCompleted();
            OnStopped();
        }
    }

}