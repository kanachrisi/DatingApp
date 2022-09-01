using API.Errors;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExpectionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExpectionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExpectionMiddleware(RequestDelegate next, ILogger<ExpectionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); //will get the context and pass it the next piece in the middleware pipelines
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);

                //We need to write the exception to the response 
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError; //500

                var response = _env.IsDevelopment()
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new ApiException(context.Response.StatusCode, "Internal Server Error");

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);
            }

        }
    }
}
