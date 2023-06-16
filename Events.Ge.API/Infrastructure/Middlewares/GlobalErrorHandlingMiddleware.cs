using Newtonsoft.Json;

namespace Events.Ge.API.Infrastructure.Middlewares
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var error = new ApiError(context, ex);
            var result = JsonConvert.SerializeObject(error);
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error.Status.Value;
            await context.Response.WriteAsync(result);
        }

    }
}
