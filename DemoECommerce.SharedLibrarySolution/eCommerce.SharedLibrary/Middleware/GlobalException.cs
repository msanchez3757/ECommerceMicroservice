using eCommerce.SharedLibrary.Logs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace eCommerce.SharedLibrary.Middleware
{
    public class GlobalException(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            string message = "Sorry, internal server error occured";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Error";

            try
            {
                await next(context);

                // check if Exception is too many requests //429 status code
                if(context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    message = "Too many requests. Please try again later.";
                    statusCode = (int)StatusCodes.Status429TooManyRequests;
                    title = "Warning";
                    await ModifyHeader(context, title, message, statusCode);
                }

                //if response is unauthorized // 401 status code
                if(context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    message = "You are not authorized to access this resource.";
                    statusCode = (int)StatusCodes.Status401Unauthorized;
                    title = "Alert";
                    await ModifyHeader(context, title, message, statusCode);
                }

                // if response is forbidden // 403 status code
                if(context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    message = "Access to this resource is forbidden.";
                    statusCode = (int)StatusCodes.Status403Forbidden;
                    title = "Out of Access";
                    await ModifyHeader(context, title, message, statusCode);
                }
            }
            catch(Exception ex)
            {
                // Log Original Exception / File, Debugger, Console
                LogException.LogExceptions(ex);

                // check if Exceptuion is Timeout // 408 status code
                if (ex is TaskCanceledException || ex is TimeoutException)
                {
                    title = "Out of time";
                    message = "Request timeout... try again";
                    statusCode = (int)StatusCodes.Status408RequestTimeout;
                }
                await ModifyHeader(context, title, message, statusCode);
            }
        }

        private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
        {
            // display scary-free message to client
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
            {
                Detail = message,
                Status = statusCode,
                Title = title
            }), CancellationToken.None);
            return;
        }
    }
}
