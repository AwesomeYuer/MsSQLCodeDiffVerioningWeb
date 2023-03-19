namespace Microshaoft.WebApi.ModelBinders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

public class JTokenModelBinder : JsonModelBinder<JToken>, IModelBinder
{
    public override JToken OnParseProcessFunc(string json)
    {
        return JToken.Parse(json)!;
    }

    public override JToken OnKeyValuePairsProcessFunc(IEnumerable<KeyValuePair<string, StringValues>> keyValuePairs)
    {
        return
             JsonWebHelper
                     .ToJToken(keyValuePairs);
    }
    public override string OnExtractJwtTokenProcessFunc(JToken jToken, string jwtTokenName)
    {
        return jToken[jwtTokenName]?.Value<string>()!;
    }
}

