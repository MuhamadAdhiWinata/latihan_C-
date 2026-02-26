using System.Net;
using System.Text.Json;
using FluentValidation;
using SharedKernel.Abstraction.Domain;

namespace SharedKernel.Abstraction.Web;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                Message = "Validation failed.",
                Errors = ex.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                })
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (DomainRuleException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";
            
            var payload = new
            {
                Message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (AppException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                ex.Message,
                ExceptionType = ex.GetType().Name
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                Message = "An unexpected error occurred.",
                Exception = ex.Message // log details instead of exposing them in production
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}