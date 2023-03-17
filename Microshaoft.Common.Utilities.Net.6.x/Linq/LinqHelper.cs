namespace Microshaoft;

public static class LinqHelper
{
    public static void ForEach<T>(this IEnumerable<T> @this, Func<int, T, bool> predict)
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            if (predict(i, t))
            {
                break;
            }
        }
    }
    public static IEnumerable<T> ForEachAsIEnumerable<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, (bool NeedBreak, bool NeedYield)>
                                                    predict
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

    public static async IAsyncEnumerable<T> ForEachAsIAsyncEnumerable<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, Task<(bool NeedBreak, bool NeedYield)>>
                                                    predictAsync
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


    public static async IAsyncEnumerable<T> ForEachAsIAsyncEnumerable<T>
                                        (
                                            this IAsyncEnumerable<T> @this
                                            , Func<int, T, Task<(bool NeedBreak, bool NeedYield)>>
                                                    predictAsync
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

    public static async Task ForEachAsync<T>
                                    (
                                        this IEnumerable<T> @this
                                        , Func<int, T, Task<bool>>
                                                        predictAsync
                                    )
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            if (await predictAsync(i, t))
            {
                break;
            }
        }
    }

    public static IEnumerable<T>
                                TakeTopFirst<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, (bool NeedYield, bool NeedBreak)> predict
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
                                        , Func<int, T, Task<(bool NeedYield, bool NeedBreak)>> predictAsync
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
                                            , Func<int, T, Task<(bool NeedYield, bool NeedBreak)>> predictAsync
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
                                            , Func<int, T, Task<(bool NeedYield, bool NeedBreak)>> predictAsync
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
                                        , Func<int, T, (bool NeedYield, bool NeedBreak)> predict
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
