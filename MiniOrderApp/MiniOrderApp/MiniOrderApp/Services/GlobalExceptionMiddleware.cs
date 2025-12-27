using System.Net;
using System.Text.Json;

namespace MiniOrderApp.Services;

public class GlobalExceptionMiddleware(RequestDelegate next)
{
        public async Task InvokeAsync(HttpContext context)
        {
                try
                {
                        await next(context);
                }
                catch (Exception ex)
                {
                        await HandleExceptionAsync(context, ex);
                }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
                context.Response.ContentType = "application/json";

                var status = HttpStatusCode.InternalServerError;
                var message = "An unexpected error occurred.";

                switch (exception)
                {
                        case ArgumentException:
                        case InvalidOperationException:
                                status = HttpStatusCode.BadRequest;
                                message = exception.Message;
                                break;

                        case KeyNotFoundException:
                                status = HttpStatusCode.NotFound;
                                message = exception.Message;
                                break;
                }

                var response = new
                {
                        StatusCode = (int)status,
                        Error = message
                };

                context.Response.StatusCode = (int)status;
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
}