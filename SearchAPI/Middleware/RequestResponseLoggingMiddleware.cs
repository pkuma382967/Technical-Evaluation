using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SearchAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
        private readonly bool _logRequestBody;
        private readonly bool _logResponseBody;

        public RequestResponseLoggingMiddleware(
            RequestDelegate next,
            IConfiguration config,
            ILogger<RequestResponseLoggingMiddleware> logger
        )
        {
            _next = next;
            _logger = logger;

            // Read settings from appsettings.json
            _logRequestBody = config.GetValue<bool>("Serilog:LogRequestBody");
            _logResponseBody = config.GetValue<bool>("Serilog:LogResponseBody");
        }

        public async Task Invoke(HttpContext context)
        {
            var requestInfo = await FormatRequest(context.Request);
            var userInfo = GetUserInfo(context);
            var headers = GetHeaders(context);

            // Log request details
            _logger.LogInformation(
                "Request: {RequestInfo} | User: {UserInfo} | Headers: {Headers}",
                requestInfo,
                userInfo,
                headers
            );

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            var responseInfo = await FormatResponse(context.Response);

            // Log response based on status
            if (context.Response.StatusCode >= 400)
            {
                _logger.LogError("Response: {ResponseInfo}", responseInfo);
            }
            else
            {
                _logger.LogInformation("Response: {ResponseInfo}", responseInfo);
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();
            var body = _logRequestBody
                ? await new StreamReader(request.Body).ReadToEndAsync()
                : "[Request Body Logging Disabled]";
            request.Body.Position = 0;

            return $"Method: {request.Method}, Path: {request.Path}, Body: {body}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var body = _logResponseBody
                ? await new StreamReader(response.Body).ReadToEndAsync()
                : "[Response Body Logging Disabled]";
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"Status: {response.StatusCode}, Body: {body}";
        }

        private string GetUserInfo(HttpContext context)
        {
            return context.User.Identity?.IsAuthenticated == true
                ? $"User: {context.User.Identity.Name}"
                : "Anonymous User";
        }

        private string GetHeaders(HttpContext context)
        {
            return string.Join("; ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"));
        }
    }
}
