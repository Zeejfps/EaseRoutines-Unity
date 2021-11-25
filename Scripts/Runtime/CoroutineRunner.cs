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
                if (m_IsRunning == value)
                    return;
                
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
        int m_ActiveRunnerIndex;
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

        /// <summary>
        /// Adds the coroutines to be executed and begins running
        /// </summary>
        /// <param name="coroutines"></param>
        /// <returns></returns>
        public CoroutineRunner Run(params IEnumerator[] coroutines)
        {
            m_IsInterrupted = false;
            m_ThenAction = null;
            
            var coroutineCount = coroutines.Length;
            var newActiveRunnerCount = m_ActiveRunnerCount + coroutineCount;
            
            while (newActiveRunnerCount > m_RunnersPool.Count)
                m_RunnersPool.Add(new CoroutineRunner(m_Target));

            for (var i = 0; i < coroutineCount; i++)
            {
                var runner = m_RunnersPool[m_ActiveRunnerCount + i];
                runner.StartCoroutine(coroutines[i]);
            }

            m_ActiveRunnerCount = newActiveRunnerCount;

            if (!IsRunning)
            {
                IsRunning = true;
                var activeRunner = m_RunnersPool[m_ActiveRunnerIndex];
                if (activeRunner.IsRunning)
                    activeRunner.Then(OnActiveRunnerCompleted);
                else
                    OnActiveRunnerCompleted();
            }

            return this;
        }

        /// <summary>
        /// Stops execution of all coroutines
        /// </summary>
        public void Interrupt()
        {
            if (m_Coroutine != null)
            {
                m_Target.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
            }

            for (var i = m_ActiveRunnerIndex; i < m_ActiveRunnerCount; i++)
                m_RunnersPool[i].Interrupt();

            m_IsInterrupted = true;
            IsRunning = false;
        }

        void OnActiveRunnerCompleted()
        {
            CoroutineRunner activeRunner;
            do
            {
                m_ActiveRunnerIndex++;
                if (m_ActiveRunnerIndex >= m_ActiveRunnerCount)
                {
                    IsRunning = false;
                    return;
                }
                activeRunner = m_RunnersPool[m_ActiveRunnerIndex];
                
            } while (!activeRunner.IsRunning);
            
            activeRunner.Then(OnActiveRunnerCompleted);
        }
        
        void StartCoroutine(IEnumerator coroutine)
        {
            IsRunning = true;
            m_ThenAction = null;
            m_IsInterrupted = false;
            m_Coroutine = m_Target.StartCoroutine(WaitForCompletion(coroutine));
        }

        void OnStopped()
        {
            m_ActiveRunnerCount = 0;
            m_ActiveRunnerIndex = 0;
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

        IEnumerator WaitForCompletion(IEnumerator coroutine)
        {
            yield return coroutine;
            IsRunning = false;
        }
    }
}