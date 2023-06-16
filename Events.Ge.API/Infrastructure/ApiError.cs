using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Events.Ge.API.Infrastructure
{
    public class ApiError : ProblemDetails
    {
        public const string UnhandlerErrorCode = "UnhandledError";
        private readonly HttpContext _httpContext;
        private readonly Exception _exception;


        public string Code { get; set; }

        public string TraceId
        {
            get
            {
                if (Extensions.TryGetValue("TraceId", out var traceId))
                {
                    return (string)traceId;
                }

                return "";
            }

            set => Extensions["TraceId"] = value;
        }

        public ApiError(HttpContext httpContext, Exception exception)
        {
            _httpContext = httpContext;
            _exception = exception;

            TraceId = httpContext.TraceIdentifier;

            //default
            Code = UnhandlerErrorCode;
            Status = (int)HttpStatusCode.InternalServerError;
            Title = exception.Message;
            Instance = httpContext.Request.Path;

            HandleException((dynamic)exception);
        }

        private void HandleException(Exception exception)
        {

        }
    }
}