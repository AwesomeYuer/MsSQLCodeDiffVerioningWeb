namespace Microshaoft;

using Newtonsoft.Json.Linq;
public static partial class JsonHelper
{

    public static JObject InplaceSortBy<TKey>
                        (
                            this JObject @this
                            , Func<JProperty, TKey> keySelector = null!
                        )
    {
        // store to list
        var jProperties = @this.Properties().ToList();
        foreach (var jProperty in jProperties)
        {
            // remove from JToken
            jProperty.Remove();
        }
        var ordered = jProperties.OrderBy(p => keySelector(p));
        foreach (var jProperty in ordered)
        {
            @this.Add(jProperty);
            var @value = jProperty.Value as JObject;
            if (@value is not null)
            {
                InplaceSortBy(@value, keySelector);
            }
            else //(jProperty.Value is JArray)
            {
                int count = jProperty.Value.Count();
                for (int i = 0; i < count; i++)
                {
                    var jToken = jProperty.Value[i];
                    if (jToken is JObject)
                    {
                        InplaceSortBy((JObject) jProperty.Value[i]!, keySelector);
                    }
                }
            }
        }
        return @this;
    }

    public static JObject Flatten
                                (
                                    this JObject @this
                                    , Func
                                            <
                                                (string Prefix, string PropertyName, int IndexInArray, string Seprator, string PrefixPropertyName)
                                                , string
                                            >
                                                onPropertySegmentsNamingProcessFunc = null!         
                                    , bool needClone = true
                                )
    {
        var result = new JObject();
        var clone = @this;
        if (needClone)
        {
            clone = (JObject) clone.DeepClone();
        }
        Flatten("", clone, result, onPropertySegmentsNamingProcessFunc);
        return result;
    }
    private static void Flatten
                            (
                                string prefix
                                , JToken jToken
                                , JObject result
                                , Func
                                        <
                                            (string Prefix, string PropertyName, int indexInArray, string Seprator, string PrefixPropertyName)
                                            , string
                                        >
                                            onPropertySegmentsNamingProcessFunc = null!
                            )
    {
        switch (jToken.Type)
        {
            case JTokenType.Object:
                var jProperties = jToken.Children<JProperty>();

                foreach (var jProperty in jProperties)
                {
                    var combined = CombinePrefix
                                    (
                                        $"{prefix}"
                                        , jProperty.Name
                                        , -1
                                        , onPropertySegmentsNamingProcessFunc
                                    );
                    Flatten
                        (
                            combined
                            , jProperty.Value
                            , result
                            , onPropertySegmentsNamingProcessFunc
                        );
                }
                break;
            case JTokenType.Array:
                int i = 0;
                var items = jToken.Children();
                foreach (var item in items)
                {
                    var combined = CombinePrefix
                                    (
                                        prefix
                                        , $"[{i}]"
                                        , i
                                        , onPropertySegmentsNamingProcessFunc
                                        , string.Empty
                                    );
                    Flatten
                        (
                             combined
                            , item
                            , result
                            , onPropertySegmentsNamingProcessFunc
                            //, string.Empty
                        );
                    i++;
                }
                break;
            default:
                result.Add(prefix, jToken);
                break;
        }
    }

    private static string CombinePrefix
                                    (
                                        string prefix
                                        , string properyName
                                        , int indexInArray
                                        , Func
                                                <
                                                    (string Prefix, string PropertyName, int IndexInArray, string Seprator, string PrefixPropertyName)
                                                    , string
                                                >
                                                    onPropertySegmentsNamingProcessFunc = null!
                                        , string seprator = "."
                                    )
    {
        var r = string.IsNullOrEmpty(prefix) ? properyName : $"{prefix}{seprator}{properyName}";
        if (onPropertySegmentsNamingProcessFunc != null)
        {
            r = onPropertySegmentsNamingProcessFunc
                        (
                            (prefix, properyName, indexInArray, seprator, r)
                        );
        }
        return r;
    }
}
