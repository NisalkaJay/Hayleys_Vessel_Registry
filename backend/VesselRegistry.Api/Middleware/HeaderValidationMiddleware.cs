using System.Text.Json;
using VesselRegistry.Api.Dtos;

namespace VesselRegistry.Api.Middleware
{
    public class HeaderValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public HeaderValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // The assessment requires checking if either header is missing[cite: 2]
            if (!context.Request.Headers.TryGetValue("X-User-Id", out var userIdStr) ||
                !context.Request.Headers.TryGetValue("X-Company-Id", out var companyIdStr))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Error(
                    errorCode: "Validation",
                    message: "Missing X-User-Id or X-Company-Id headers."
                );

                var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
                return; // Return 400 immediately[cite: 2, 3]
            }

            // Parse and store them in HttpContext.Items so services can access them later[cite: 2]
            if (int.TryParse(userIdStr, out int userId) && int.TryParse(companyIdStr, out int companyId))
            {
                context.Items["UserId"] = userId;
                context.Items["CompanyId"] = companyId;
            }

            await _next(context);
        }
    }
}