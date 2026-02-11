using Microsoft.AspNetCore.Mvc;

namespace StructuredOutput.Application;

public static class Module
{
    public static void Register(this IEndpointRouteBuilder app)
    {
        app.MapPost("/run/image", async (
            [FromServices] Service service,
            [FromQuery] string userInput,
            [FromForm] IFormFile file) =>
        {
            var response = await service.RunWithImageAsync(userInput, file.OpenReadStream());
            return Results.Ok(response);
        }).DisableAntiforgery();

        app.MapPost("/run/image-url", async (
            [FromServices] Service service,
            [FromQuery] string userInput,
            [FromQuery] string url,
            [FromQuery] string mediaType) =>
        {
            var response = await service.RunWithImageUriAsync(
                userInput,
                url,
                mediaType);
            return Results.Ok(response);
        });
        
        app.MapPost("/run/structured-output", async (
            [FromServices] Service service,
            [FromQuery] string userInput,
            [FromForm] IFormFile file) =>
        {
            var response = await service.RunWithStructuredOutputStreaming(userInput, file.OpenReadStream());
            return Results.Ok(response);
        }).DisableAntiforgery();
    }
}
