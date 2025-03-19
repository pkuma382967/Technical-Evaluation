namespace SearchAPI.Middleware
{
    /// <summary>
    /// Logging requests and responses
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestResponseLoggingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation(
                "Request: {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );
            await _next(context);
            _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
        }
    }
}
