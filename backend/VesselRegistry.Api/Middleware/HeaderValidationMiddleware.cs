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
            if (!context.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) ||
                !context.Request.Headers.TryGetValue("X-Company-Id", out var companyIdHeader) ||
                !int.TryParse(userIdHeader, out var userId) ||
                !int.TryParse(companyIdHeader, out var companyId) ||
                userId <= 0 ||
                companyId <= 0)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Error(
                    errorCode: "Validation",
                    message: "X-User-Id and X-Company-Id headers are required and must be positive integers."
                );

                var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
                return; // Return 400 immediately[cite: 2, 3]
            }

            context.Items["UserId"] = userId;
            context.Items["CompanyId"] = companyId;

            await _next(context);
        }
    }
}