#if NETCOREAPP
namespace Microshaoft.HttpBatchHandler.Events
{
    using System;
    using Microsoft.AspNetCore.Http;
    public class BatchRequestExecutedContext
    {
        /// <summary>
        ///     Abort after this request?
        /// </summary>
        public bool Abort { get; set; }

        /// <summary>
        ///     Exception
        /// </summary>
        public Exception Exception { get; set; } = null!;

        /// <summary>
        ///     The individual HttpRequest
        /// </summary>
        public HttpRequest Request { get; set; } = null!;

        /// <summary>
        ///     The individual HttpResponse
        /// </summary>
        public HttpResponse Response { get; set; } = null!;

        /// <summary>
        ///     State
        /// </summary>
        public object State { get; set; } = null!;
    }
}
#endif