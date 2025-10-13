using System.Text;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.RequestHelpers
{
    [AttributeUsage(AttributeTargets.All)]
    public class CacheAttribute(int timeToLiveSeconds) : Attribute, IAsyncActionFilter
    {//IAsyncActionFilter is used to create custom action filters in ASP.NET Core that can execute code before and after an action method is called.
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // قبل تنفيذ الـ Action
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse)) {
                var contentResult = new ContentResult
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;// النتيجة - إذا عينتها توقف تنفيذ الـ Action
                return;
            }

            // تنفيذ الـ Action
            var executedContext = await next();

            // بعد تنفيذ الـ Action
            if (executedContext.Result is OkObjectResult okObjectResult)
            {
                if (okObjectResult.Value != null)
                    await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(timeToLiveSeconds));
            }

        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append($"{request.Path}");
            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
