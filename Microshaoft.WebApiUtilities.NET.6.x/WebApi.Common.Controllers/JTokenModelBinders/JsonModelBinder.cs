namespace Microshaoft.WebApi.ModelBinders;

using Microshaoft.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public abstract class JsonModelBinder<T>
                                    : IModelBinder
                                            where T : class
{
    public const string ItemKeyOfRequestJsonParameters = "requestJsonParameters";

    protected virtual string JwtTokenKey { get; set; } = "xJwtToken";

    public abstract string OnExtractJwtTokenProcessFunc(T tJson, string jwtTokenName);

    public abstract T OnParseProcessFunc(string json);

    public abstract T OnKeyValuePairsProcessFunc
                                (
                                     IEnumerable
                                        <KeyValuePair<string, StringValues>>
                                            keyValuePairs
                                );

    public T GetJsonModelBindingResult
                                (
                                    ModelBindingContext bindingContext
                                )
        
    {
        var httpContext = bindingContext
                                    .HttpContext;
        var request = httpContext
                                .Request;
        var ok = false;
        ok = bindingContext
                .TryParseObjectJsonParameters<T>
                    (
                        OnParseProcessFunc
                        , out T parameters
                        , out var secretJwtToken
                        , OnKeyValuePairsProcessFunc
                        , JwtTokenKey
                        , OnExtractJwtTokenProcessFunc
                    );
        if (ok)
        {
            if 
                (
                    parameters is not null
                    &&
                    !httpContext
                            .Items
                            .ContainsKey
                                (
                                    ItemKeyOfRequestJsonParameters
                                )
                )
            {
                
                httpContext
                        .Items
                        .Add
                            (
                                ItemKeyOfRequestJsonParameters
                                , parameters
                            );
            }
        }
        if (!StringValues.IsNullOrEmpty(secretJwtToken))
        {
            httpContext
                    .Items
                    .Add
                        (
                            JwtTokenKey
                            , secretJwtToken
                        );
        }
        return parameters!;
    }

    public virtual JToken GetJTokenModelBindingResult(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext
                                    .HttpContext;
        var request = httpContext
                                .Request;
        IConfiguration configuration =
                    (IConfiguration)httpContext
                                            .RequestServices
                                            .GetService
                                                (
                                                    typeof(IConfiguration)
                                                )!;
        var jwtTokenName = configuration
                                    .GetSection("TokenName")
                                    .Value;
        var ok = false;
        ok = request
                .TryParseObjectJsonParameters<JToken>
                    (
                        (x) => JToken.Parse(x)
                        , out JToken parameters
                        , out var secretJwtToken
                        , jwtTokenName: jwtTokenName!
                    );
        if (ok)
        {
            if
                (
                    !httpContext
                            .Items
                            .ContainsKey
                                (
                                    ItemKeyOfRequestJsonParameters
                                )
                )
            {
                httpContext
                        .Items
                        .Add
                            (
                                ItemKeyOfRequestJsonParameters
                                , parameters
                            );
            }
        }
        if (!StringValues.IsNullOrEmpty(secretJwtToken))
        {
            httpContext
                    .Items
                    .Add
                        (
                            jwtTokenName!
                            , secretJwtToken
                        );
        }
        return parameters;
    }

    public virtual Task BindModelAsync(ModelBindingContext bindingContext)
    {

        var parameters = GetJsonModelBindingResult
                                        (
                                            bindingContext
                                        );
        bindingContext
                    .Result = ModelBindingResult
                                            .Success
                                                (
                                                    parameters
                                                );
        return
            Task
                .CompletedTask;
    }
}

