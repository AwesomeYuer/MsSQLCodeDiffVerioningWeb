namespace Microshaoft;

public class Debouncer<T> : IDisposable
{
    private readonly CancellationTokenSource cts = new CancellationTokenSource();
    private readonly TimeSpan waitTime;
    private int counter;

    public Debouncer(TimeSpan? waitTime = null)
    {
        waitTime = waitTime ?? TimeSpan.FromSeconds(3);
    }

    public void Debounce(Action<T> action, T state)
    {
        var current = Interlocked.Increment(ref counter);

        Task.Delay(waitTime).ContinueWith(task =>
        {
            // Is this the last task that was queued?
            if (current == counter && !cts.IsCancellationRequested)
                action(state);
#if NETSTANDARD2_0
              task.Dispose();
#endif

        }, cts.Token);
    }

    public void Dispose()
    {
        cts.Cancel();
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
