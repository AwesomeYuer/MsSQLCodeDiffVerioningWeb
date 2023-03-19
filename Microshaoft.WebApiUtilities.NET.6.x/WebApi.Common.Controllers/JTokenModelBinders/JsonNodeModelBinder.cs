namespace Microshaoft.WebApi.ModelBinders;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Text.Json.Nodes;
public class JsonNodeModelBinder : JsonModelBinder<JsonNode> ,IModelBinder
{
    //private const string _itemKeyOfRequestJTokenParameters = "requestJTokenParameters";

    protected override string JwtTokenKey 
                { get => "xJwtToken" ; }

    public override JsonNode OnParseProcessFunc(string json)
    {
        return JsonNode.Parse(json)!;
    }

    public override JsonNode OnKeyValuePairsProcessFunc(IEnumerable<KeyValuePair<string, StringValues>> keyValuePairs)
    {
        return
             JsonWebHelper
                     .ToJsonNode(keyValuePairs);
    }
    public override string OnExtractJwtTokenProcessFunc(JsonNode jsonNode, string jwtTokenName)
    {
        return jsonNode[jwtTokenName]?.GetValue<string>()!;
    }
}

