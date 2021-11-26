using System;
using System.Runtime.CompilerServices;

namespace EnvDev
{
    public static class CoroutineRunnerExt
    {
        public static CoroutineRunnerAwaiter GetAwaiter(this CoroutineRunner runner)
        {
            return new CoroutineRunnerAwaiter(runner);
        }
    }
    
    public class CoroutineRunnerAwaiter : INotifyCompletion
    {
        readonly CoroutineRunner m_Runner;
        
        public CoroutineRunnerAwaiter(CoroutineRunner runner)
        {
            m_Runner = runner;
            m_Runner.Completed += Runner_OnCompleted;
        }

        void Runner_OnCompleted()
        {
            m_Runner.Completed -= Runner_OnCompleted;
            IsCompleted = true;
        }

        public bool IsCompleted { get; private set; }

        public void GetResult()
        {
            m_Runner.Completed -= Runner_OnCompleted;
            m_Runner.Interrupt();
            IsCompleted = true;
        }

        public void OnCompleted(Action continuation)
        {
            IsCompleted = true;
            continuation();
        }
    }
}