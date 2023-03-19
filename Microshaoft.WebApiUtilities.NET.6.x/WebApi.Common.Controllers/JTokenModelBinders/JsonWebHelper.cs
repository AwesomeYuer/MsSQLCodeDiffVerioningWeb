//#if NETCOREAPP
namespace Microshaoft.WebApi;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;

public static class JsonWebHelper
{
    public static bool IsJsonRequest(this HttpRequest @this)
    {
        return @this.ContentType == "application/json";
    }
    public static T ToObjectJson<T>
                            (
                                this IQueryCollection @this
                                , Func
                                        <
                                            IEnumerable
                                                    <KeyValuePair<string, StringValues>>
                                            , T
                                        >
                                    onReturnProcessFunc
                            )
    {
        return
             ToObjectJson
                (
                    (IEnumerable<KeyValuePair<string, StringValues>>)
                        @this
                        , onReturnProcessFunc
                );
    }
    public static T ToObjectJson<T>
                            (
                                this IFormCollection @this
                                , Func
                                        <
                                            IEnumerable
                                                    <KeyValuePair<string, StringValues>>
                                            , T
                                        >
                                    onReturnProcessFunc
                            )
    {
        return
             ToObjectJson
                (
                    (IEnumerable<KeyValuePair<string, StringValues>>)
                        @this
                    , onReturnProcessFunc
                );
    }
    public static T ToObjectJson<T>
                            (
                                this
                                    IEnumerable
                                        <KeyValuePair<string, StringValues>>
                                            @this
                                   , Func
                                        <
                                            IEnumerable
                                                    <KeyValuePair<string, StringValues>>
                                            , T
                                        >
                                    onReturnProcessFunc
                            )
    {
        return
            onReturnProcessFunc(@this);
    }

    public static JsonNode ToJsonNode
                                (
                                    this
                                        IEnumerable
                                            <KeyValuePair<string, StringValues>>
                                                @this
                                )
    {
        var keyValuePairs =
                        @this
                            .Select
                                (
                                    (x) =>
                                    {
                                        JsonNode jsonNode = null!;
                                        if (x.Value.Count > 1)
                                        {
                                            var jsonArray = new JsonArray();
                                            foreach (var v in x.Value)
                                            {
                                                jsonArray.Add(v);
                                            }
                                            jsonNode = jsonArray;
                                        }
                                        else
                                        {
                                            var valueText = x.Value[0];
                                            if (!string.IsNullOrEmpty(valueText))
                                            {
                                                jsonNode = valueText!;
                                            }
                                        }
                                        return
                                            (x.Key, jsonNode);
                                    }
                                );
        JsonObject result = null!;
        foreach (var (key, @value) in keyValuePairs)
        {
            result ??= new JsonObject();
            result.Add(key, @value);
        }
        return result;
    }

    public static JToken ToJToken
                        (
                            this
                                IEnumerable
                                    <KeyValuePair<string, StringValues>>
                                        @this
                        )
    {
        IEnumerable<JProperty>
                    jProperties =
                                @this
                                    .Select
                                        (
                                            (x) =>
                                            {
                                                JToken jToken = null!;
                                                if (x.Value.Count > 1)
                                                {
                                                    jToken = new JArray(x.Value!);
                                                }
                                                else
                                                {
                                                    var valueText = x.Value[0];
                                                    if (!string.IsNullOrEmpty(valueText))
                                                    {
                                                        jToken = new JValue(x.Value[0]);
                                                    }
                                                }
                                                return
                                                        new
                                                            JProperty
                                                                (
                                                                    x.Key
                                                                    , jToken
                                                                );
                                            }
                                        );
        var result = new JObject(jProperties);
        return result;
    }
}
//#endif