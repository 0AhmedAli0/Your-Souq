using Microsoft.AspNetCore.Http;

namespace API.Errors
{
    public class ApiErrorResponse(int errorCode, string errorMessage, string? details)
    {
        public int StatusCode { get; set; } = errorCode;
        public string Message { get; set; } = errorMessage;//?? SetCustomErrorMessage(errorCode);
        public string? Details { get; set; } = details;

        private string SetCustomErrorMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "You are not Authorized",
                404 => "Resource not found",
                500 => "Validation Error",
                _ => string.Empty //like default
            };
        }
    }
}
