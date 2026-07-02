using Fgc.Catalog.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Fgc.Catalog.API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (UnauthorizedException ex)
            {
                _logger.LogWarning(ex,
                    "Unauthorized access | Path={Path}",
                    context.Request.Path);

                await WriteErrorAsync(
                    context,
                    HttpStatusCode.Unauthorized,
                    ex.Message);
            }
            catch (ConflictException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Conflict | Path={Path}",
                    context.Request.Path
                );

                await WriteErrorAsync(
                    context,
                    HttpStatusCode.Conflict,
                    ex.Message
                );
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex,
                    "Resource not found | Path={Path}",
                    context.Request.Path);

                await WriteErrorAsync(
                    context,
                    HttpStatusCode.NotFound,
                    ex.Message);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex,
                    "Invalid request | Path={Path}",
                    context.Request.Path);

                await WriteErrorAsync(
                    context,
                    HttpStatusCode.BadRequest,
                    ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "EXCEPTION TYPE: {Type} | MESSAGE: {Message}",
                    ex.GetType().FullName,
                    ex.Message);
            }
        }

        private static async Task WriteErrorAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string message)
        {
            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var response = new
            {
                error = message,
                traceId = context.TraceIdentifier
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}

