using System.Runtime.CompilerServices;

namespace JsonBasedLocalization.Web.Middlewares
{
    public static class RequestCultureMiddlewareExtension
    {
        public static IApplicationBuilder UseRequestCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCultureMiddleware>();
        }
    }
}
