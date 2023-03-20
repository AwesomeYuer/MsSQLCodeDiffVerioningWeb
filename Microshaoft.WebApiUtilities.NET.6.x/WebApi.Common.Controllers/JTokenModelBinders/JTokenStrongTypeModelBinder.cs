using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microshaoft.WebApi.ModelBinders;

public class JTokenStrongTypeModelBinder<T> : JsonModelBinder<JToken>
{
    public override Task BindModelAsync(ModelBindingContext bindingContext)
    {
        JToken jToken = GetJsonModelBindingResult(bindingContext);
        var json = jToken.ToString();
        var result = JsonConvert.DeserializeObject<T>(json);
        bindingContext
                   .Result = ModelBindingResult
                                           .Success
                                               (
                                                   result
                                               );
        return
               Task
                   .CompletedTask;
    }

    public override string OnExtractJwtTokenProcessFunc(JToken jToken, string jwtTokenName)
    {
        var r = string.Empty;
        if (jToken is not JArray)
        {
            r = jToken[jwtTokenName]?.Value<string>()!;
        }
        return r;
    }

    public override JToken OnKeyValuePairsProcessFunc(IEnumerable<KeyValuePair<string, StringValues>> keyValuePairs)
    {
        return
             JsonWebHelper
                     .ToJToken(keyValuePairs);
    }

    public override JToken OnParseProcessFunc(string json)
    {
        return JToken.Parse(json)!;
    }
}
