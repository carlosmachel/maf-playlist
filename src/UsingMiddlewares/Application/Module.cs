using Microsoft.AspNetCore.Mvc;

namespace UsingMiddlewares.Application;

public static class Module
{
    public static void Register(this IEndpointRouteBuilder app)
    {
        app.MapPost("/ai-agent", async (
            [FromServices] Service service,
            [FromQuery] string userInput) =>
        {
            var response = await service.RunAsync(userInput);
            return Results.Ok(response);
        });
        
        app.MapPost("/ai-agent/with-guardrail", async (
            [FromServices] Service service,
            [FromQuery] string userInput) =>
        {
            var response = await service.RunWithWordingGuardrailAsync(userInput);
            return Results.Ok(response);
        });
        
        app.MapPost("/ai-agent/with-function-calling-middleware", async (
            [FromServices] Service service,
            [FromQuery] string userInput) =>
        {
            var response = await service.RunWithFunctionCallingMiddlewareAsync(userInput);
            return Results.Ok(response);
        });
        
        app.MapPost("/ai-agent/with-function-calling-middleware-overide", async (
            [FromServices] Service service,
            [FromQuery] string userInput) =>
        {
            var response = await service.RunWithFunctionCallingOverrideMiddlewareAsync(userInput);
            return Results.Ok(response);
        });
    }
}
