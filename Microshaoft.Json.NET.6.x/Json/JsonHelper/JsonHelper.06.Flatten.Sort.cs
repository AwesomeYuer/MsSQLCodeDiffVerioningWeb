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
                        InplaceSortBy((JObject)jProperty.Value[i]!, keySelector);
                    }
                }
            }
        }
        return @this;
    }

    public static JObject Flatten(this JObject @this, bool needClone = true)
    {
        var result = new JObject();
        var clone = @this;
        if (needClone)
        {
            clone = (JObject)clone.DeepClone();
        }
        Flatten("", clone, result);
        return result;
    }
    private static void Flatten(string prefix, JToken jToken, JObject result)
    {
        switch (jToken.Type)
        {
            case JTokenType.Object:
                var jProperties = jToken.Children<JProperty>();

                foreach (var jProperty in jProperties)
                {
                    Flatten(CombinePrefix($"{prefix}", jProperty.Name), jProperty.Value, result);
                }
                break;
            case JTokenType.Array:
                int i = 0;
                var items = jToken.Children();
                foreach (var item in items)
                {
                    Flatten(CombinePrefix(prefix, $"[{i}]", ""), item, result);
                    i++;
                }
                break;
            default:
                result.Add(prefix, jToken);
                break;
        }
    }

    private static string CombinePrefix(string prefix, string name, string seprator = ".")
    {
        return string.IsNullOrEmpty(prefix) ? name : $"{prefix}{seprator}{name}";
    }
}
