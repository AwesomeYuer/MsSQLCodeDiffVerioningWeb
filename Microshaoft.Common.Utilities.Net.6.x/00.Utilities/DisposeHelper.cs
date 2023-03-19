namespace Microshaoft;

public static class DisposeHelper
{
    public static bool CheckDisposed<T>
                        (
                            this IDisposable @this
                            , Action<T> checkProcessAction
                        )
                            where T : IDisposable
    {
        var r = false;
        try
        {
            checkProcessAction((T) @this);
        }
        catch (ObjectDisposedException)
        {
            r = true;
        }
        return r;
    }

    public static bool CheckAsyncDisposed<T>
                                (
                                    this IAsyncDisposable @this
                                    , Action<T> checkProcessAction
                                )
                                        where T : IAsyncDisposable
    {
        var r = false;
        try
        {
            checkProcessAction((T) @this);
        }
        catch (ObjectDisposedException)
        {
            r = true;
        }
        return r;
    }
}
