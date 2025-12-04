using URLShortener.Exceptions;

namespace URLShortener.Middlewares
{
    public class ExceptionHandlingMiddleware(RequestDelegate _next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException exception)
            {
                await HandleExceptionAsync(context, StatusCodes.Status404NotFound, exception.Message);
            }
            catch (CustomValidationException exception)
            {
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, exception.Message);
            }
            catch (ForbiddenException exception)
            {
                await HandleExceptionAsync(context, StatusCodes.Status403Forbidden, exception.Message);
            }
            catch (ConflictException exception)
            {
                await HandleExceptionAsync(context, StatusCodes.Status409Conflict, exception.Message);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var result = System.Text.Json.JsonSerializer.Serialize(new { error = message });
            await context.Response.WriteAsync(result);
        }
    }
}
