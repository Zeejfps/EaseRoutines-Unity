using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnvDev
{
    public class CoroutineRunner
    {
        /// <summary>
        /// True if the runner has any coroutines that are still running
        /// </summary>
        public bool IsRunning => m_IsRunning;
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
            if (!m_IsRunning)
            {
                m_IsRunning = true;
                var activeRunner = m_RunnersPool[m_ActiveRunnerIndex];
                if (activeRunner.IsRunning)
                    activeRunner.Completed = UpdateActiveRunner;
                else
                    UpdateActiveRunner();
            }
            
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
            OnInterrupted();
        }

        void UpdateActiveRunner()
        {
            var activeRunner = m_RunnersPool[m_ActiveRunnerIndex];
            activeRunner.Completed = null;
            do
            {
                m_ActiveRunnerIndex++;
                if (m_ActiveRunnerIndex >= m_ActiveRunnerCount)
                {
                    OnCompleted();
                    return;
                }
                activeRunner = m_RunnersPool[m_ActiveRunnerIndex];
                
            } while (!activeRunner.IsRunning);
            
            activeRunner.Completed = UpdateActiveRunner;
        }

        void OnInterrupted()
        {
            m_IsInterrupted = true;
            Reset();
        }

        void OnCompleted()
        {
            Reset();
            m_ThenAction?.Invoke();
        }

        void Reset()
        {
            m_IsRunning = false;
            m_ActiveRunnerCount = 0;
            m_ActiveRunnerIndex = 0;
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
            Completed = null;
            m_Coroutine = m_Target.StartCoroutine(WaitForCompletion(coroutine));
        }

        public void StopCoroutine()
        {
            m_Target.StopCoroutine(m_Coroutine);
            IsRunning = false;
        }
        
        IEnumerator WaitForCompletion(IEnumerator coroutine)
        {
            yield return coroutine;
            IsRunning = false;
            OnCompleted();
        }

        void OnCompleted()
        {
            Completed?.Invoke();
        }
    }
}