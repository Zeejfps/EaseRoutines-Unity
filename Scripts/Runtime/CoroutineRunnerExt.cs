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
        }
        
        public bool IsCompleted { get; private set; }

        public bool GetResult() => !m_Runner.IsRunning;

        public void OnCompleted(Action continuation)
        {
            IsCompleted = true;
            continuation();
        }
    }
}