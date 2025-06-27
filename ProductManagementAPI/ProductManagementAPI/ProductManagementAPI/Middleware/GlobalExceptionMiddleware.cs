using ProductManagementAPI.Models.Responses;
using System.Net;
using System.Text.Json;

namespace ProductManagementAPI.Middleware
{
    public class GlobalExceptionMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            catch(Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                ArgumentException => new ApiResponse<object>
                {
                    Success = false,
                    Message = exception.Message,
                    Errors = new List<string> { exception.Message }
                },
                InvalidOperationException => new ApiResponse<object>
                {
                    Success = false,
                    Message = exception.Message,
                    Errors = new List<string> { exception.Message }
                },
                _ => new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while processing your request",
                    Errors = new List<string> { "Internal server error" }
                }
            };

            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
