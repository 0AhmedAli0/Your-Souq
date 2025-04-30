using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
    {
        public async Task InvokeAsync(HttpContext context)
        {
			try
			{
				await next.Invoke(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex, env);
			}
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex, IHostEnvironment env)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var ServerExceptionMessage = env.IsDevelopment() ?
                new ApiErrorResponse(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new ApiErrorResponse(context.Response.StatusCode, ex.Message, "Internal server error");

            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };// السطر دا علشان طريقه الكتابه مش اكتر ولا اقل ولو عايز تشتالها اشتالها
            var json = JsonSerializer.Serialize(ServerExceptionMessage, options);

            return context.Response.WriteAsync(json);
        }
    }
}
