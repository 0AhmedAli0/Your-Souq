using Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InvalidateCache(string pattern) : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            // If the action was successful, remove the cache
            if(resultContext.Exception == null || resultContext.ExceptionHandled)
            {
                var cacheService = resultContext.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
                await cacheService.RemoveCachedResponseAsync(pattern);
            }

        }
    }
}
