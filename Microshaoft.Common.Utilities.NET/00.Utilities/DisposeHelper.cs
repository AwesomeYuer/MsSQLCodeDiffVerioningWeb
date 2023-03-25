namespace Microshaoft;

public static class DisposeHelper
{
    public static bool CheckDisposed<T>
                        (
                            this IDisposable @this
                            , Action<T> onCheckProcessAction
                        )
                            where T : IDisposable
    {
        var r = false;
        try
        {
            onCheckProcessAction((T) @this);
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
                            , Action<T> onCheckProcessAction
                        )
                                where T : IAsyncDisposable
    {
        var r = false;
        try
        {
            onCheckProcessAction((T) @this);
        }
        catch (ObjectDisposedException)
        {
            r = true;
        }
        return r;
    }
}
