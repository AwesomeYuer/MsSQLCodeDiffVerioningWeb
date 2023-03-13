namespace Microshaoft;

public static class LinqHelper
{
    public static void ForEach<T>(this IEnumerable<T> @this, Func<int, T, bool> eachProcessFunc)
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            if (eachProcessFunc(i, t))
            {
                break;
            }
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> @this, Func<int, T, Task<bool>> eachProcessAsync)
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            if (await eachProcessAsync(i, t))
            {
                break;
            }
        }
    }

    public static IEnumerable<T>
                                TakeTopFirst<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, (bool, bool)> predict
                                        )
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            (bool needYield, bool needBreak)
                                    = predict(i, t);

            if (needYield)
            {
                yield return t;
            }
            if (needBreak)
            {
                break;
            }
        }
    }

    public static IEnumerable<T>
                            TakeTopFirst<T>
                                    (
                                        this IEnumerable<T> @this
                                        , Func<int, T, Task<(bool, bool)>> predictAsync
                                    )
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            (bool needYield, bool needBreak)
                                    = predictAsync(i, t).Result;
            if (needYield)
            {
                yield return t;
            }
            if (needBreak)
            {
                break;
            }
        }
    }

    public static async IAsyncEnumerable<T>
                                TakeTopFirstAsync<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, Task<(bool, bool)>> predictAsync
                                        )
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            (bool needYield, bool needBreak)
                                    = await predictAsync(i, t);
            if (needYield)
            {
                yield return t;
            }
            if (needBreak)
            {
                break;
            }
        }
    }

    public static async IAsyncEnumerable<T> 
                                TakeTopFirstAsync<T>
                                        (
                                            this IAsyncEnumerable<T> @this
                                            , Func<int, T, Task<(bool, bool)>> predictAsync
                                        )
    {
        var i = 0;
        await foreach (T t in @this)
        {
            i++;
            (bool needYield, bool needBreak)
                                    = await predictAsync(i, t);
            if (needYield)
            {
                yield return t;
            }
            if (needBreak)
            {
                break;
            }
        }
    }

    public static async IAsyncEnumerable<T>
                            TakeTopFirstAsync<T>
                                    (
                                        this IAsyncEnumerable<T> @this
                                        , Func<int, T, (bool, bool)> predict
                                    )
    {
        var i = 0;
        await foreach (T t in @this)
        {
            i++;
            (bool needYield, bool needBreak)
                                    = predict(i, t);
            if (needYield)
            {
                yield return t;
            }
            if (needBreak)
            {
                break;
            }
        }
    }
}