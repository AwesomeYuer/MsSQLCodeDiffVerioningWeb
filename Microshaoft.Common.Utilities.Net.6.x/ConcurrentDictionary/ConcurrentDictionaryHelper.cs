namespace Microshaoft
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    public static partial class ConcurrentDictionaryHelper
    {
        public static TValue Add<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key, TValue @value)
            where TKey : notnull
        {
            TValue result = @this.AddOrUpdate(key, @value, (k, v) => { return @value; });
            return result;
        }

        public static TValue Update<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key, TValue @value)
            where TKey : notnull
        {
            TValue result = @this
                                .AddOrUpdate
                                    (
                                        key
                                        , @value
                                        , (k, v) =>
                                        {
                                            return @value;
                                        }
                                    );
            return result;
        }

        public static TValue Get<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key)
            where TKey : notnull
        {
            @this.TryGetValue(key, out TValue? @value);
            return @value!;
        }

        public static TValue Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> @this, TKey key)
            where TKey : notnull
        {
            _ = @this.TryRemove(key, out TValue? @value);
            return @value!;
        }
        public static void ForEach<TKey, TValue>
                                (
                                    this ConcurrentDictionary<TKey, TValue> @this
                                    , Func<TKey, TValue, bool> processFunc
                                )
            where TKey : notnull
        {
            foreach (KeyValuePair<TKey, TValue> kvp in @this)
            {
                if 
                    (
                        processFunc(kvp.Key, kvp.Value)
                    )
                {
                   break;
                }
            }
        }
    }
}
