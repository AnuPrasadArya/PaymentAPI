using Serilog;
using Serilog.Context;

namespace PaymentAPI.API.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Add RequestPath as a log context property
            using (LogContext.PushProperty("RequestPath", path))
            {
                // Log request
                context.Request.EnableBuffering();
                var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;

                Log.Information("Request: {Method} {Path} \nBody: {Body}",
                    context.Request.Method,
                    path,
                    requestBody);

                // Capture response
                var originalBody = context.Response.Body;
                using var newBody = new MemoryStream();
                context.Response.Body = newBody;

                await _next(context);

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                Log.Information("Response: {StatusCode} \nBody: {ResponseBody}",
                    context.Response.StatusCode,
                    responseText);

                await newBody.CopyToAsync(originalBody);
            }
        }
    }
}
