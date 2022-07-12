namespace Microshaoft
{
    using Microshaoft.WebApi.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ActionConstraints;
    using Microsoft.AspNetCore.Mvc.Controllers;
    using System;
    
    public class ConfigurableActionConstraint<TRouteAttribute>
                                :
                                    IActionConstraint
                                    , IActionConstraintMetadata
                                    , IServiceProvider
                    where
                        TRouteAttribute
                                : RouteAttribute, IConfigurable
    {
        private readonly TRouteAttribute _routeAttribute;
        public ConfigurableActionConstraint
                    (
                        TRouteAttribute
                            routeAttribute
                        , Func
                                <
                                    ConfigurableActionConstraint<TRouteAttribute>
                                    , ActionConstraintContext
                                    , TRouteAttribute
                                    , bool
                                >
                            onAcceptCandidateActionProcessFunc = null!
                    )
        {
            _routeAttribute = routeAttribute;
            _onAcceptCandidateActionProcessFunc
                        = onAcceptCandidateActionProcessFunc;
        }
        public virtual bool OnAcceptAsyncOrSyncCandidateActionSelectorProcessFunc
                                    (
                                        ConfigurableActionConstraint<TRouteAttribute> configurableActionConstraint
                                        , ActionConstraintContext actionConstraintContext
                                        , TRouteAttribute routeAttribute
                                    )
        {
            return
                OnAcceptAsyncOrSyncCandidateActionSelectorProcessFunc
                        <AbstractStoreProceduresExecutorControllerBase>
                            (
                                configurableActionConstraint
                                , actionConstraintContext
                                , routeAttribute
                            );
        }
        protected virtual bool OnAcceptAsyncOrSyncCandidateActionSelectorProcessFunc<TControllerType>
                                    (
                                        ConfigurableActionConstraint<TRouteAttribute> configurableActionConstraint
                                        , ActionConstraintContext actionConstraintContext
                                        , TRouteAttribute routeAttribute
                                    )
        {
            var r = false;
            if (actionConstraintContext.Candidates.Count > 1)
            {
                var httpContext = actionConstraintContext.RouteContext.HttpContext;
                var request = httpContext.Request;
                var currentCandidateAction = actionConstraintContext
                                                            .CurrentCandidate
                                                            .Action;
                var currentControllerActionDescriptor = ((ControllerActionDescriptor)currentCandidateAction);
                var methodParamsLength = currentControllerActionDescriptor
                                                    .MethodInfo
                                                    .GetParameters()
                                                    .Length;
                var currentControllerType = currentControllerActionDescriptor.ControllerTypeInfo.AsType();
                var routeContext = actionConstraintContext.RouteContext;

                var actionRoutePath = string.Empty;
                if
                    (
                        routeContext
                                .RouteData
                                .Values
                                .TryGetValue
                                        (
                                            " "
                                            , out var @value
                                        )
                    )
                {
                    if (@value != null)
                    {
                        actionRoutePath = @value.ToString()!.ToLower();
                    }
                }
                var urlPath = request.Path.ToString();
                //=====================================================================================
                var httpMethod = $"Http{request.Method}";
                var accessingConfigurationKey = "DefaultAccessing";
                if (request.Path.ToString().Contains("/export/", StringComparison.OrdinalIgnoreCase))
                {
                    accessingConfigurationKey = "exporting";
                }
                var isAsyncExecutingOnDemand =
                        _routeAttribute
                                    .Configuration
                                    .GetOrDefaultValue
                                        (
                                            $"Routes:{actionRoutePath}:{httpMethod}:{accessingConfigurationKey}:isAsyncExecuting"
                                            , false
                                        );
                //======================================================================================
                var isAsyncMethod = currentControllerActionDescriptor
                                                                .MethodInfo
                                                                .IsAsync();
                if (typeof(TControllerType).IsAssignableFrom(currentControllerType))
                {
                    if (request.Method == "GET")
                    {
                        var hasQueryString = request.QueryString.HasValue;
                        if
                            (
                                hasQueryString
                                && methodParamsLength > 0
                                && isAsyncExecutingOnDemand == isAsyncMethod
                            )
                        {
                            if
                                (
                                    !actionRoutePath!.IsNullOrEmptyOrWhiteSpace()
                                    &&
                                    urlPath.EndsWith(actionRoutePath, StringComparison.OrdinalIgnoreCase)
                                )
                            {
                                r = true;
                            }
                            else if (actionRoutePath!.IsNullOrEmptyOrWhiteSpace())
                            {
                                r = true;
                            }

                        }
                        else if
                            (
                                !hasQueryString
                                && methodParamsLength == 0
                                && isAsyncExecutingOnDemand == isAsyncMethod
                            )
                        {
                            r = true;
                        }
                    }
                    else
                    {
                        var hasContentType =
                                            (
                                                request.HasFormContentType
                                                ||
                                                request.HasJsonContentType()
                                            );
                        var hasContentLength = request.ContentLength > 0;
                        if
                            (
                                hasContentType
                                && hasContentLength
                                && methodParamsLength > 0
                                && isAsyncExecutingOnDemand == isAsyncMethod
                            )
                        {
                            if
                                (
                                    !actionRoutePath!.IsNullOrEmptyOrWhiteSpace()
                                    &&
                                    urlPath.EndsWith(actionRoutePath)
                                )
                            {
                                r = true;
                            }
                            else if (actionRoutePath!.IsNullOrEmptyOrWhiteSpace())
                            {
                                r = true;
                            }
                        }
                        else if
                            (
                                hasContentType
                                && !hasContentLength
                                && methodParamsLength == 0
                                && isAsyncExecutingOnDemand == isAsyncMethod
                            )
                        {
                            r = true;
                        }
                    }

                }
            }
            else
            {
                r = true;
            }
            return
                    r;
        }

        private readonly
                        Func
                            <
                                ConfigurableActionConstraint<TRouteAttribute>
                                , ActionConstraintContext
                                , TRouteAttribute
                                , bool
                            >
                                _onAcceptCandidateActionProcessFunc = null!;
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
            //return
            //    this;
        }

        public bool Accept(ActionConstraintContext context)
        {
            bool r;
            if (_onAcceptCandidateActionProcessFunc != null)
            {
                r = _onAcceptCandidateActionProcessFunc
                            (
                                this
                                , context
                                , _routeAttribute
                            );
            }
            else
            {
                r = OnAcceptAsyncOrSyncCandidateActionSelectorProcessFunc
                            (
                                this
                                , context
                                , _routeAttribute
                            );
            }
            return
                    r;
        }
    }
}
