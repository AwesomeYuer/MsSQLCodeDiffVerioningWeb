namespace Microshaoft;

public static class LinqHelper
{
    public static void ForEach<T>(this IEnumerable<T> @this, Func<int, T, bool> predict = null!)
    {
        var i = 0;
        var needBreak = false;
        foreach (T t in @this)
        {
            i++;
            if (predict != null)
            {
                needBreak = predict(i, t);
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
                                                    predictAsync = null!
                                )
    {
        Func<int, T, bool> predict = null!;
        if (predictAsync != null)
        {
            predict = new Func<int, T, bool>
                        (
                            (i, t) =>
                            {
                                return predictAsync(i, t).Result;
                            }
                        );
        }
        ForEach(@this, predict);
        await Task.CompletedTask;
    }

    // ForEachAsIEnumerable 可以替代 TakeTopFirst, TakeTopFirst 没用了
    // ForEachAsIEnumerable 同时替代了 Select Where

    public static IEnumerable<(TSource Source, TResult Result)>
                                    ForEachAsIEnumerable<TSource, TResult>
                                                (
                                                    this IEnumerable<TSource> @this
                                                    , Func
                                                            <
                                                                int
                                                                , TSource
                                                                , (bool NeedYield, bool NeedBreak, TResult Result)
                                                            >
                                                                predict = null!
                                                )
    {
        TResult result = default!;

        var i = 0;
        bool needYield = true;
        foreach (TSource source in @this)
        {
            i++;
            bool needBreak = false;
            if (predict != null)
            {
                (
                    needYield
                    , needBreak
                    , result
                )
                    = predict(i, source);
            }
            if (needYield)
            {
                yield
                    return
                        (source, result!);
            }
            if (needBreak)
            {
                break;
            }
        }
    }

    public static IEnumerable<(TSource Source, TResult Result)>
                                    ForEachAsIEnumerable<TSource, TResult>
                                                (
                                                    this IEnumerable<TSource> @this
                                                    , Func
                                                            <
                                                                int
                                                                , TSource
                                                                , Task<(bool NeedYield, bool NeedBreak, TResult Result)>
                                                            >
                                                                predictAsync = null!
                                                )
    {
        Func<int, TSource, (bool NeedYield, bool NeedBreak, TResult Result)> predict = null!;
        if (predictAsync != null)
        {
            predict = new Func<int, TSource, (bool NeedYield, bool NeedBreak, TResult Result)>
                        (
                            (i, source) =>
                            {
                                return
                                    predictAsync
                                            (i, source)
                                                    .Result;
                            }
                        );
        }
        return
            ForEachAsIEnumerable
                    (
                        @this
                        , predict
                    );
    }

    public static async IAsyncEnumerable<(TSource Source, TResult Result)>
                                    ForEachAsIAsyncEnumerableSyncAsync<TSource, TResult>
                                                    (
                                                        this IEnumerable<TSource> @this
                                                        , Func
                                                                <
                                                                    int
                                                                    , TSource
                                                                    , (bool NeedYield, bool NeedBreak, TResult Result)
                                                                >
                                                                    predict = null!
                                                    )
    {
        var i = 0;
        bool needYield = true;
        TResult result = default!;
        foreach (TSource source in @this)
        {
            i++;
            bool needBreak = false;
            if (predict != null)
            {
                (needYield, needBreak, result)
                                        = predict(i, source);
            }
            if (needYield)
            {
                yield return await
                                Task
                                    .FromResult
                                        (
                                            (source, result)
                                        );
            }
            if (needBreak)
            {
                break;
            }
        }

    }

    public static async IAsyncEnumerable<(TSource, TResult)>
                            ForEachAsIAsyncEnumerableAsyncAsync<TSource, TResult>
                                            (
                                                this IEnumerable<TSource> @this
                                                , Func
                                                        <
                                                            int
                                                            , TSource
                                                            , Task<(bool NeedYield, bool NeedBreak, TResult result)>
                                                        >
                                                            predictAsync = null!
                                            )
    {
        Func<int, TSource, (bool NeedYield, bool NeedBreak, TResult result)> predict = null!;
        if (predictAsync != null)
        {
            predict = new Func<int, TSource, (bool NeedYield, bool NeedBreak, TResult result)>
                        (
                            (i, source) =>
                            {
                                return
                                    predictAsync(i, source).Result;
                            }
                        );
        }
        var i = 0;
        bool needYield = true;
        TResult result = default!;
        foreach (TSource source in @this)
        {
            i++;
            bool needBreak = false;
            if (predict != null)
            {
                (needYield, needBreak, result)
                                    = await predictAsync!(i, source);
            }
            if (needYield)
            {
                yield return
                            await
                                Task
                                    .FromResult
                                        (
                                            (source, result)
                                        );
            }
            if (needBreak)
            {
                break;
            }
        }
    }
       



    private static IEnumerable<T>
                                TakeTopFirst<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, (bool NeedYield, bool NeedBreak)>
                                                    predict
                                        )
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;

            var (needYield, needBreak) = predict(i, t);
            
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

    private static IEnumerable<T>
                            TakeTopFirst<T>
                                    (
                                        this IEnumerable<T> @this
                                        , Func<int, T, Task<(bool NeedYield, bool NeedBreak)>>
                                                predictAsync
                                    )
    {
        var i = 0;

        foreach (T t in @this)
        {
            i++;
            
            var (needYield, needBreak)
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

    private static async IAsyncEnumerable<T>
                                TakeTopFirstAsync<T>
                                        (
                                            this IEnumerable<T> @this
                                            , Func<int, T, Task<(bool NeedYield, bool NeedBreak)>>
                                                    predictAsync
                                        )
    {
        var i = 0;
        foreach (T t in @this)
        {
            i++;
            var (needYield, needBreak)
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

    private static async IAsyncEnumerable<T>
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
            var (needYield, needBreak)
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

    private static async IAsyncEnumerable<T>
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
            var (needYield, needBreak)
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
