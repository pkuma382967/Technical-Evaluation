﻿using System.Collections.Concurrent;

namespace SearchAPI.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        private static readonly ConcurrentDictionary<string, List<DateTime>> _requestLogs =
            new ConcurrentDictionary<string, List<DateTime>>();

        private readonly HashSet<string> _blacklistedIPs;
        private readonly int _requestLimit;
        private readonly int _timeWindowSeconds;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IConfiguration config,
            ILogger<RateLimitingMiddleware> logger
        )
        {
            _next = next;
            _config = config;
            _logger = logger;

            _requestLimit = _config.GetValue<int>("RateLimiting:RequestLimit", 5);
            _timeWindowSeconds = _config.GetValue<int>("RateLimiting:TimeWindowSeconds", 10);

            var blacklistedIPs =
                _config.GetSection("BlacklistedIPs").Get<List<string>>() ?? new List<string>();
            _blacklistedIPs = new HashSet<string>(blacklistedIPs);
        }

        public async Task Invoke(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            if (
                context.Request.Path.StartsWithSegments("/swagger")
                || context.Request.Path.StartsWithSegments("/swagger/v1/swagger.json")
            )
            {
                await _next(context);
                return;
            }

            if (_blacklistedIPs.Contains(ip))
            {
                _logger.LogWarning($"Blocked request from blacklisted IP: {ip}");
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync(
                    $"Access forbidden for this {ip}. Please contact administrator."
                );
                return;
            }

            bool isRateLimited = false;
            lock (_requestLogs)
            {
                if (!_requestLogs.ContainsKey(ip))
                {
                    _requestLogs[ip] = new List<DateTime>();
                }

                var requestTimes = _requestLogs[ip];

                requestTimes.RemoveAll(time =>
                    (DateTime.UtcNow - time).TotalSeconds > _timeWindowSeconds
                );

                if (requestTimes.Count >= _requestLimit)
                {
                    isRateLimited = true;
                }
                else
                {
                    requestTimes.Add(DateTime.UtcNow);
                }
            }

            if (isRateLimited)
            {
                _logger.LogWarning($"Too many requests from IP: {ip}");
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
                return;
            }

            await _next(context);
        }
    }
}
