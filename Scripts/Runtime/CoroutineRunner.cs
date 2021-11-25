using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EnvDev
{
    public class CoroutineRunner
    {
        /// <summary>
        /// True if the runner has any coroutines that are running
        /// </summary>
        bool m_IsRunning;
        public bool IsRunning
        {
            get => m_IsRunning;
            private set
            {
                m_IsRunning = value;
                if (!m_IsRunning)
                    OnStopped();
            }
        }

        readonly MonoBehaviour m_Target;
        readonly List<CoroutineRunner> m_RunnersPool = new List<CoroutineRunner>();

        Action m_ThenAction;
        Coroutine m_Coroutine;
        int m_ActiveRunnerCount;
        int m_RunningCoroutineIndex;
        bool m_IsInterrupted;

        public CoroutineRunner(MonoBehaviour target)
        {
            m_Target = target;
        }

        /// <summary>
        /// Sets an action that will be executed when all coroutines are complete
        /// </summary>
        /// <param name="action">Code to run when all coroutines are complete</param>
        public void Then(Action action)
        {
            m_ThenAction = action;
        }

        public CoroutineRunner Run(params IEnumerator[] coroutines)
        {
            m_IsInterrupted = false;
            m_ThenAction = null;
            
            var coroutineCount = coroutines.Length;
            var newActiveRunnerCount = m_ActiveRunnerCount + coroutineCount;
            
            while (newActiveRunnerCount > m_RunnersPool.Count)
                m_RunnersPool.Add(new CoroutineRunner(m_Target));

            for (var i = 0; i < coroutineCount; i++)
                m_RunnersPool[m_ActiveRunnerCount + i].StartCoroutine(WaitFor(coroutines[i]));

            m_ActiveRunnerCount = newActiveRunnerCount;

            if (!IsRunning) StartCoroutine(WaitForAll());

            return this;
        }

        /// <summary>
        /// Stops execution of all coroutines
        /// </summary>
        public void Interrupt()
        {
            Assert.IsTrue(IsRunning, "IsRunning");

            m_Target.StopCoroutine(m_Coroutine);

            for (var i = m_RunningCoroutineIndex; i < m_ActiveRunnerCount; i++)
                m_RunnersPool[i].Interrupt();

            m_IsInterrupted = true;
            IsRunning = false;
        }

        void StartCoroutine(IEnumerator coroutine)
        {
            IsRunning = true;
            m_Coroutine = m_Target.StartCoroutine(coroutine);
        }

        void OnStopped()
        {
            m_ActiveRunnerCount = 0;
            m_RunningCoroutineIndex = 0;
            if (!m_IsInterrupted)
                OnCompleted();
            else
                OnInterrupted();
        }

        void OnInterrupted()
        {
            // TODO: Add an event maybe?
        }

        void OnCompleted()
        {
            m_ThenAction?.Invoke();
        }

        IEnumerator WaitFor(IEnumerator coroutine)
        {
            yield return coroutine;
            IsRunning = false;
        }
        
        IEnumerator WaitForAll()
        {
            while (IsRunning)
            {
                if (m_RunnersPool[m_RunningCoroutineIndex].IsRunning)
                {
                    yield return null;
                }
                else
                {
                    m_RunningCoroutineIndex++;
                    if (m_RunningCoroutineIndex >= m_ActiveRunnerCount)
                        IsRunning = false;
                }
            }
        }
    }

}