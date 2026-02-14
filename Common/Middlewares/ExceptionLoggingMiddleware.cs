using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Common.Middlewares
{
    public class ExceptionLoggingMiddleware
    {
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _environment;
        public ExceptionLoggingMiddleware(ILogger<ExceptionLoggingMiddleware> logger, RequestDelegate next, IHostEnvironment environment)
        {
            _logger = logger;
            _next = next;
            _environment = environment;
        }
        
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleUnexpectedException(context, ex);
            }
        }
        
        private async Task HandleUnexpectedException(HttpContext context, Exception exception)
        {
            LogException(exception);

            if (_environment.IsDevelopment())
            {
                var sb = new StringBuilder();
                sb.Append("500 Internal Server Error").AppendLine();
                sb.Append(exception.Message).AppendLine();
                sb.Append("--------------------------------------------------").AppendLine();
                sb.Append($"Exception type: {exception.GetType().FullName}").AppendLine();
                sb.Append("Stack trace:").AppendLine();
                sb.Append("--------------------------------------------------").AppendLine();
                sb.Append(exception.StackTrace).AppendLine();
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(sb.ToString());
            }
            else
            {
                await WriteErrorToResponse(context, HttpStatusCode.InternalServerError, "Internal server error occured");
            }
        }
        
        private static async Task WriteErrorToResponse(HttpContext context, HttpStatusCode code, object error)
        {
            var errorText = JsonSerializer.Serialize(error);
            context.Response.StatusCode = (int)code;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(errorText);
        }

        private void LogException(Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            var inner = exception.InnerException;
            if (inner != null)
            {
                _logger.LogError(inner, inner.Message);
            }
        }
    }
}
