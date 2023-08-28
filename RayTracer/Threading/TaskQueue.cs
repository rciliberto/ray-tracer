namespace RayTracer.Threading;

/// <summary>
///     A class to queue tasks and wait for them to complete.
/// </summary>
public class TaskQueue
{
    /// <summary>
    ///     A manual reset event to signal when all tasks have completed.
    /// </summary>
    private readonly ManualResetEvent _taskQueueCompleteEvent = new(false);

    /// <summary>
    ///     The number of tasks remaining in the queue.
    /// </summary>
    private int _remainingTasks;

    /// <summary>
    ///     The total number of tasks in the queue.
    /// </summary>
    private int _totalTasks;

    /// <summary>
    ///     Add a task to the queue.
    /// </summary>
    /// <param name="callback">The task to add to the queue.</param>
    public void Enqueue(WaitCallback callback)
    {
        _totalTasks++;
        Interlocked.Increment(ref _remainingTasks);
        ThreadPool.QueueUserWorkItem(state =>
        {
            callback(state);
            if (Interlocked.Decrement(ref _remainingTasks) == 0)
                _taskQueueCompleteEvent.Set();
        });
    }

    /// <summary>
    ///     Wait for all tasks in the queue to complete.
    /// </summary>
    public void WaitForCompletion()
    {
        _taskQueueCompleteEvent.WaitOne();
    }

    /// <summary>
    ///     Get the progress of the queue as a value between 0 and 1.
    /// </summary>
    /// <returns>the progress of the queue.</returns>
    public double GetProgress()
    {
        return 1 - (double)_remainingTasks / _totalTasks;
    }
}