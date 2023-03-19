namespace Microshaoft.Web;

using Microshaoft.WebApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;

public static partial class HttpRequestHelper
{
    public static JsonResult EchoJsonRequestJsonResult<T>
                    (
                       this HttpRequest @this
                        , T requestJsonParameters
                        , object context
                    )
    {
        return
            new JsonResult
            (
                new
                {
                    WebApiRequestJsonParameters = requestJsonParameters
                    ,
                    context
                    ,
                    Request = new
                    {
                        @this.ContentLength
                            ,
                        @this.ContentType
                            ,
                        @this.Cookies
                            ,
                        @this.HasFormContentType
                            ,
                        HasJsonContentType = @this.HasJsonContentType()
                            ,
                        @this.Headers
                            ,
                        @this.Host
                            ,
                        @this.IsHttps
                            ,
                        @this.Method
                            ,
                        @this.Path
                            ,
                        @this.PathBase
                            ,
                        @this.Protocol
                            ,
                        @this.Query
                            ,
                        @this.QueryString
                            ,
                        HasQueryStringValue = @this.QueryString.HasValue
                            ,
                        @this.RouteValues
                            ,
                        @this.Scheme
                    }
                    ,
                    HttpContext = new
                    {
                        Connection = new
                        {
                            RemoteIpAddress = @this
                                                    .HttpContext
                                                    .Connection
                                                    .RemoteIpAddress!
                                                    .ToString()
                        }
                            //, HttpContext.Items
                            ,
                        User = new
                        {
                            Claims = @this
                                        .HttpContext
                                        .User
                                        .Claims
                                        .ToArray()
                                ,
                            Identity = new
                            {
                                @this
                                    .HttpContext
                                    .User
                                    .Identity!
                                    .Name
                            }
                        }
                    }
                }
            );
    }



    public static JsonResult NewJsonResult
                            (
                                int statusCode
                                , int resultCode
                                , string message
                            )
    {
        return
            new JsonResult
                    (
                        new
                        {
                            statusCode
                            ,
                            resultCode
                            ,
                            message
                        }
                    )
            {
                StatusCode = statusCode
                    ,
                ContentType = "application/json"
            };
    }

    public static void SetJsonResult
                            (
                                this ActionExecutingContext @this
                                , int statusCode
                                , int resultCode
                                , string message
                            )
    {
        @this
            .Result = NewJsonResult
                        (
                            statusCode
                            , resultCode
                            , message
                        );
    }
    public static void SetJsonResult
                    (
                        this ActionExecutedContext @this
                        , int statusCode
                        , int resultCode
                        , string message
                    )
    {
        @this
            .Result = NewJsonResult
                        (
                            statusCode
                            , resultCode
                            , message
                        );
    }
    public static void SetNotFoundJsonResult
                            (
                                this ActionExecutedContext @this
                                , int? resultCode = null
                                , string message = null!
                            )
    {

        var statusCode = 404;
        if (resultCode == null || !resultCode.HasValue)
        {
            resultCode = -1 * statusCode;
        }
        if (message.IsNullOrEmptyOrWhiteSpace())
        {
            var request = @this.HttpContext.Request;
            message = $"{request.Path.Value} not found!";
        }
        @this
            .Result = NewJsonResult
                        (
                            statusCode
                            , resultCode.Value
                            , message
                        );
    }
    public static void SetNotFoundJsonResult
                    (
                        this ActionExecutingContext @this
                        , int? resultCode = null
                        , string message = null!
                    )
    {
        var statusCode = 404;
        if (resultCode == null || !resultCode.HasValue)
        {
            resultCode = -1 * statusCode;
        }
        if (message.IsNullOrEmptyOrWhiteSpace())
        {
            var request = @this.HttpContext.Request;
            message = $"{request.Path.Value} not found!";
        }
        @this
            .Result = NewJsonResult
                        (
                            statusCode
                            , resultCode.Value
                            , message
                        );
    }

    public static string GetActionRoutePath
                (
                    this HttpRequest @this
                    , string key = " "
                )
    {
        return
            @this
                .RouteValues[key]!
                .ToString()!;
    }

    public static string GetActionRoutePathOrDefault
        (
            this HttpRequest @this
            , string defaultValue = default!
            , string key = " "
        )
    {
        string? r;
        if
            (
                TryGetActionRoutePath
                    (
                        @this
                        , out var @value
                        , key
                    )
            )
        {
            r = @value;
        }
        else
        {
            r = defaultValue;
        }
        return
            r;
    }
    public static bool TryGetActionRoutePath
        (
            this HttpRequest @this
            , out string @value
            , string key = " "
        )
    {
        @value = string.Empty;
        var r = false;
        if
            (
                @this
                    .RouteValues
                    .TryGetValue
                        (
                            key
                            , out var @object
                        )
                &&
                @object != null
            )
        {
            if (@object is string s)
            {
                if (!s.IsNullOrEmptyOrWhiteSpace())
                {
                    @value = s;
                    r = true;
                }
            }
        }
        return
            r;
    }
    public static bool TryParseObjectJsonParameters<T>
        (
            this HttpRequest @this
            , Func<string, T> onParseProcessFunc
            , out T parameters
            , out string secretJwtToken
            , Func
                <
                    IEnumerable
                        <
                            KeyValuePair<string, StringValues>
                        >
                        , T
                >
                    onKeyValuePairsProcessFunc = null!
            , ModelBindingContext modelBindingContext = null!
            , string jwtTokenName = "xJwtToken"
            , Func<T, string, string> onExtractJwtTokenProcessFuncAsync = null!

        )
                                    where T : class
    {
        bool r;
        T result = default!;
        if (modelBindingContext is not null)
        { 
        
        
        }

        void requestFormBodyProcess()
        {
            var hasContentLength = @this.ContentLength > 0;
            if
                (
                    !@this.IsJsonRequest()
                    &&
                    @this.HasFormContentType
                    &&
                    hasContentLength
                )
            {

                if 
                    (
                        modelBindingContext is not null
                        &&
                        onKeyValuePairsProcessFunc is not null
                    )
                {
                    result = modelBindingContext
                                        .GetFormObjectJsonAsync(onKeyValuePairsProcessFunc).Result;
                }
            }
            else if (hasContentLength)
            {
                using var streamReader = new StreamReader(@this.Body);
                var json = streamReader.ReadToEnd();
                if (!json.IsNullOrEmptyOrWhiteSpace())
                {
                    r = json
                            .IsJson
                                (
                                    onParseProcessFunc
                                    , out result
                                    , true
                                );
                }
            }
        }
        void requestQueryStringHeaderProcess()
        {
            var queryString = @this.QueryString.Value;
            if (queryString!.IsNullOrEmptyOrWhiteSpace())
            {
                return;
            }
            queryString = HttpUtility
                                .UrlDecode
                                    (
                                        queryString
                                    );
            if (queryString!.IsNullOrEmptyOrWhiteSpace())
            {
                return;
            }
            queryString = queryString!.TrimStart('?');
            if (queryString.IsNullOrEmptyOrWhiteSpace())
            {
                return;
            }
            var isJson = false;
            try
            {
                if (queryString.IsJson(onParseProcessFunc, out result, true))
                {
                    isJson = result is T;
                }
            }
            catch
            {

            }
            if (!isJson)
            {
                if (onKeyValuePairsProcessFunc != null)
                {
                    result = @this.Query.ToObjectJson(onKeyValuePairsProcessFunc);
                }
                //Console.WriteLine("@this.Query.ToJToken()");
            }
        }
        // 取 jwtToken 优先级顺序：Header → QueryString → Body
        StringValues jwtToken = string.Empty;
        var needExtractJwtToken = !jwtTokenName.IsNullOrEmptyOrWhiteSpace();

        if (needExtractJwtToken)
        {
            secretJwtToken = (jwtToken = @this.Headers[jwtTokenName.ToLower()]).ToString();
            needExtractJwtToken = secretJwtToken.IsNullOrEmptyOrWhiteSpace();
        }


        void extractJwtToken()
        {
            if (needExtractJwtToken)
            {
                if (result != null)
                {
                    if (StringValues.IsNullOrEmpty(jwtToken))
                    {
                        //var j = jsonNode[jwtTokenName];
                        //if (j != null)
                        //{
                        //    jwtToken = j.GetValue<string>();
                        //}
                        if (needExtractJwtToken)
                        {
                            if (onExtractJwtTokenProcessFuncAsync != null)
                            {
                                jwtToken = onExtractJwtTokenProcessFuncAsync(result, jwtTokenName);
                            }
                        }
                    }
                }
            }
        }
        if (needExtractJwtToken)
        {
            @this
                .Headers
                .TryGetValue
                        (
                           jwtTokenName
                           , out jwtToken
                        );
        }
        if
            (
                string.Compare(@this.Method, "get", true) != 0
                &&
                string.Compare(@this.Method, "head", true) != 0
            )
        {
            requestFormBodyProcess();
        }
        else
        {
            requestQueryStringHeaderProcess();
        }
        extractJwtToken();
        parameters = result;
        secretJwtToken = jwtToken!;
        r = true;
        return r;
    }

    public static bool TryParseObjectJsonParameters<T>
        (
            this ModelBindingContext @this
            , Func<string, T> onParseProcessFunc
            , out T parameters
            , out string secretJwtToken
            , Func
                <
                    IEnumerable
                        <
                            KeyValuePair<string, StringValues>
                        >
                        , T
                >
                    onKeyValuePairsProcessFunc = null!
            , string jwtTokenName = "xJwtToken"
            , Func<T, string, string> onExtractJwtTokenProcessFuncAsync = null!

        )
                                    where T : class
    {
        return
            TryParseObjectJsonParameters
                        (
                            @this.HttpContext.Request
                            , onParseProcessFunc
                            , out parameters
                            , out secretJwtToken
                            , onKeyValuePairsProcessFunc
                            , @this
                            , jwtTokenName
                            , onExtractJwtTokenProcessFuncAsync
                        );
    }


    public static async
                Task<T> GetFormObjectJsonAsync<T>
                    (
                        this ModelBindingContext @this
                        , Func
                                <
                                    IEnumerable
                                            <KeyValuePair<string, StringValues>>
                                    , T
                                >
                                onReturnProcessFunc
                    )
    {
        T r = default!;
        var formCollectionModelBinder =
                            new FormCollectionModelBinder
                                    (
                                        NullLoggerFactory
                                                    .Instance
                                    );
        await
            formCollectionModelBinder
                        .BindModelAsync(@this);
        if
            (
                @this
                    .Result
                    .IsModelSet
            )
        {
            r = JsonWebHelper
                            .ToObjectJson
                                (
                                    (IFormCollection)
                                        @this
                                            .Result
                                            .Model!
                                    , onReturnProcessFunc
                                );
        }
        return r;
    }
}
