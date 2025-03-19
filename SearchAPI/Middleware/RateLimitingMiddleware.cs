namespace SearchAPI.Middleware
{
    /// <summary>
    /// We will use middleware to implement rate limiting and validate incoming
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, DateTime> _requestTimes =
            new Dictionary<string, DateTime>();

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress.ToString();

            if (
                _requestTimes.ContainsKey(ip)
                && (DateTime.Now - _requestTimes[ip]).TotalSeconds < 1
            )
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            _requestTimes[ip] = DateTime.Now;
            await _next(context);
        }
    }
}
