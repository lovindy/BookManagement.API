using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace BookManagement.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            ErrorResponse response;

            switch (exception)
            {
                case SqlException sqlEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponse(
                        "Database error occurred.",
                        sqlEx.Message,
                        sqlEx.Number
                    );
                    break;

                case ValidationException validationEx:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new ErrorResponse(
                        "Validation error occurred.",
                        validationEx.Message
                    );
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new ErrorResponse(
                        "An error occurred while processing your request.",
                        exception.Message
                    );
                    break;
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    // Error response models
    public class ErrorResponse
    {
        public ErrorDetail Error { get; set; }

        public ErrorResponse(string message, string details, int? errorCode = null)
        {
            Error = new ErrorDetail
            {
                Message = message,
                Details = details,
                ErrorCode = errorCode
            };
        }
    }

    public class ErrorDetail
    {
        public string Message { get; set; }
        public string Details { get; set; }
        public int? ErrorCode { get; set; }
    }
}