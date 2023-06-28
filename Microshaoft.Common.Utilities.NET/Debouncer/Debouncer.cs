namespace Microshaoft;

public class Debouncer<T> : IDisposable
{
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly TimeSpan _waitTime;
    private int counter;

    public Debouncer(TimeSpan? waitTime = null)
    {
        _waitTime = waitTime ?? TimeSpan.FromSeconds(3);
    }

    public void Debounce(Action<T> action, T state)
    {
        var current = Interlocked.Increment(ref counter);

        Task.Delay(_waitTime).ContinueWith(task =>
        {
            // Is this the last task that was queued?
            if (current == counter && !_cancellationTokenSource.IsCancellationRequested)
                action(state);
#if NETSTANDARD2_0
              task.Dispose();
#endif

        }, _cancellationTokenSource.Token);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }
}

public class Debouncer : Debouncer<object>
{
    public Debouncer(TimeSpan? waitTime = null) : base(waitTime)
    {
    }

    public void Debounce(Action action)
    {
        Debounce
            (
                (x) =>
                {
                    action();
                }
                , null!
            );
    }
    //public void Dispose()
    //{
    //    Dispose();
    //}
}
