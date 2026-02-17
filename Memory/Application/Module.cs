using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Memory.Application;

public static class Module
{
    public static void Register(this IEndpointRouteBuilder app)
    {
        app.MapPost("/run", async (
            [FromServices] Service service,
            [FromQuery] string userInput,
            [FromQuery] string? currentState) =>
        {

            var state = string.IsNullOrWhiteSpace(currentState)
                ? (JsonElement?)null
                : JsonElement.Parse(currentState);

            var response = await service.RunAsync(
                userInput,
                state);
            return Results.Ok(response);
        });
        
        app.MapPost("/run-no-provider", async (
            [FromServices] Service service,
            [FromQuery] string userInput,
            [FromQuery] string? currentState) =>
        {

            var state = string.IsNullOrWhiteSpace(currentState)
                ? (JsonElement?)null
                : JsonElement.Parse(currentState);

            var response = await service.RunWithoutChatHistoryProvider(
                userInput,
                state);
            return Results.Ok(response);
        });
        
        app.MapPost("/run-user-memory", async (
            [FromServices] Service service,
            [FromQuery] string userInput,
            [FromQuery] string? currentState) =>
        {

            var state = string.IsNullOrWhiteSpace(currentState)
                ? (JsonElement?)null
                : JsonElement.Parse(currentState);

            var response = await service.RunAiContextProviderAsync(
                userInput,
                state);
            return Results.Ok(response);
        });
        
        app.MapPost("/run-both", async (
            [FromServices] Service service,
            [FromQuery] string userInput,
            [FromQuery] string? currentState) =>
        {

            var state = string.IsNullOrWhiteSpace(currentState)
                ? (JsonElement?)null
                : JsonElement.Parse(currentState);

            var response = await service.RunWithBothAsync(
                userInput,
                state);
            return Results.Ok(response);
        });
    }
}