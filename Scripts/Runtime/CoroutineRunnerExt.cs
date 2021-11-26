using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EnvDev
{
    public static class CoroutineRunnerExt
    {
        public static TaskAwaiter<bool> GetAwaiter(this CoroutineRunner runner)
        {
            return new CoroutineRunnerTaskCompletionSource(runner).Task.GetAwaiter();
        }
    }
    
    class CoroutineRunnerTaskCompletionSource : TaskCompletionSource<bool>
    {
        CoroutineRunner m_Runner;
        
        public CoroutineRunnerTaskCompletionSource(CoroutineRunner runner)
        {
            if (!runner.IsRunning)
            {
                TrySetResult(runner.IsInterrupted);
                return;
            }

            m_Runner = runner;
            m_Runner.Completed += Runner_OnCompleted;
        }

        void Runner_OnCompleted()
        {
            m_Runner.Completed -= Runner_OnCompleted;
            var result = m_Runner.IsInterrupted;
            m_Runner = null;
            TrySetResult(result);
        }
    }
}