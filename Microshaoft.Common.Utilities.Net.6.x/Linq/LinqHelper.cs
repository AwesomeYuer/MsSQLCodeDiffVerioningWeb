namespace Microshaoft;

public static class LinqHelper
{
    public static void ForEach<T>(this IEnumerable<T> @this, Func<int, T, bool> eachProcessFunc)
    {
        var i = 0;
        foreach (T x in @this)
        {
            i++;
            if (eachProcessFunc(i, x))
            {
                break;
            }
        }
    }

    public static IEnumerable<T>
                                TakeTopFirst<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, bool> predict
                                        )
    {
        var i = 0;
        foreach (T x in @this)
        {
            i++;
            yield return x;
            if (predict(i, x))
            {
                break;
            }
        }
    }

    public static IEnumerable<T>
                            TakeTopFirst<T>
                                    (
                                        this IEnumerable<T> @this
                                        , Func<int, T, Task<bool>> predictAsync
                                    )
    {
        var i = 0;
        foreach (T x in @this)
        {
            i++;
            yield return x;
            bool b;
            if (b = predictAsync(i, x).Result)
            {
                break;
            }
        }
    }

    public static async IAsyncEnumerable<T>
                                TakeTopFirstAsync<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, Task<bool>> predictAsync
                                        )
    {
        var i = 0;
        foreach (T x in @this)
        {
            i++;
            yield return x;
            bool b;
            if (b = await predictAsync(i, x))
            {
                break;
            }
        }
    }

    public static async IAsyncEnumerable<T> 
                                TakeTopFirstAsync<T>
                                        (
                                            this IAsyncEnumerable<T> @this
                                            , Func<int, T, Task<bool>> predictAsync
                                        )
    {
        var i = 0;
        await foreach (T x in @this)
        {
            i++;
            yield return x;
            bool b;
            if (b = await predictAsync(i, x))
            {
                break;
            }
        }
    }

    public static async IAsyncEnumerable<T>
                            TakeTopFirstAsync<T>
                                    (
                                        this IAsyncEnumerable<T> @this
                                        , Func<int, T, bool> predict
                                    )
    {
        var i = 0;
        await foreach (T x in @this)
        {
            i++;
            yield return x;
            bool b;
            if (b = predict(i, x))
            {
                break;
            }
        }
    }
}