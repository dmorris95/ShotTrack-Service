using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ShotTrackService
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                return;
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = 400;
                httpContext.Response.Headers.Add("ExceptionType", ex.GetType().Name);
                httpContext.Response.ContentType = "application/json";

                var exceptionResponse = new ExceptionResponseResource
                {
                    ExceptionType = ex.GetType().Name,
                    ExceptionMessage = ex.Message
                };

                var exceptionResponseJson = JsonConvert.SerializeObject(exceptionResponse);

                await httpContext.Response.WriteAsync(exceptionResponseJson);
            }
        }
    }

    internal sealed class ExceptionResponseResource
    {
        public string Timestamp => DateTime.Now.ToString();
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
