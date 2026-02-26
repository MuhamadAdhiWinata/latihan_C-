namespace SharedKernel.Abstraction.Web;

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}