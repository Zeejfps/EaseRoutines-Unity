using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EnvDev
{
    public class CoroutineRunner
    {
        public event Action Stopped;
        
        /// <summary>
        /// True if the runner has any coroutines that are running
        /// </summary>
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

        public bool IsInterrupted => m_IsInterrupted;

        readonly MonoBehaviour m_Target;
        readonly List<SingleCoroutineRunner> m_RunnersPool = new List<SingleCoroutineRunner>();

        Action m_ThenAction;
        int m_ActiveRunnerCount;
        int m_ActiveRunnerIndex;
        bool m_IsInterrupted;
        bool m_IsRunning;

        public CoroutineRunner(MonoBehaviour target)
        {
            m_Target = target;
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
                m_RunnersPool.Add(new SingleCoroutineRunner(m_Target));

            for (var i = 0; i < coroutineCount; i++)
            {
                var runner = m_RunnersPool[m_ActiveRunnerCount + i];
                runner.StartCoroutine(coroutines[i]);
            }

            m_ActiveRunnerCount = newActiveRunnerCount;

            if (!m_IsRunning)
            {
                m_IsRunning = true;
                var activeRunner = m_RunnersPool[m_ActiveRunnerIndex];
                if (activeRunner.IsRunning)
                    activeRunner.Completed = CheckIfAllRunnersAreCompleted;
                else
                    CheckIfAllRunnersAreCompleted();
            }

            return this;
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
        /// Stops execution of all coroutines
        /// </summary>
        public void Interrupt()
        {
            for (var i = m_ActiveRunnerIndex; i < m_ActiveRunnerCount; i++)
                m_RunnersPool[i].StopCoroutine();

            m_IsInterrupted = true;
            IsRunning = false;
        }

        void CheckIfAllRunnersAreCompleted()
        {
            SingleCoroutineRunner activeRunner;
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
            
            activeRunner.Completed = CheckIfAllRunnersAreCompleted;
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
            Stopped?.Invoke();
        }

        void OnCompleted()
        {
            if (m_ThenAction != null)
                m_ThenAction.Invoke();
            
            if (!IsRunning)
                Stopped?.Invoke();
        }
    }

    class SingleCoroutineRunner
    {
        public bool IsRunning;
        public Action Completed;

        readonly MonoBehaviour m_Target;
        Coroutine m_Coroutine;

        public SingleCoroutineRunner(MonoBehaviour target)
        {
            m_Target = target;
        }
        
        public void StartCoroutine(IEnumerator coroutine)
        {
            IsRunning = true;
            m_Coroutine = m_Target.StartCoroutine(WaitForCompletion(coroutine));
        }

        public void StopCoroutine()
        {
            m_Target.StopCoroutine(m_Coroutine);
            OnCompleted();
        }
        
        IEnumerator WaitForCompletion(IEnumerator coroutine)
        {
            yield return coroutine;
            OnCompleted();
        }

        void OnCompleted()
        {
            IsRunning = false;
            Completed?.Invoke();
            Completed = null;
        }
    }
}