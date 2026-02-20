using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Streaming.Application;

public static class Module
{
    public static void Register(this IEndpointRouteBuilder app)
    {
        app.MapPost("/run", async (
            [FromServices] Service service,
            [FromQuery] string userInput) =>
        {
            var response = await service.RunAsync(userInput);
            return Results.Ok(response);
        });
        
        app.MapPost("/run-streaming", async (
            HttpContext context,
            [FromServices] Service service,
            [FromQuery] string userInput) =>
        {
            context.Response.Headers.Append("Content-Type", "text/event-stream");
            context.Response.Headers.Append("Cache-Control", "no-cache");
            context.Response.Headers.Append("Connection", "keep-alive");

            var cancellationToken = context.RequestAborted;

            try
            {
                await service.RunStreamingAsync(
                    userInput, async result =>
                    {
                        if (cancellationToken.IsCancellationRequested) return;
                        
                        try
                        {
                            var data = JsonSerializer.Serialize(new 
                            { 
                                eventName = result.GetType().Name, 
                                content = result
                            }, JsonSerializerOptions.Web);
                            
                            await context.Response.WriteAsync($"data: {data}\n\n", cancellationToken);
                            await context.Response.Body.FlushAsync(cancellationToken);
                        }
                        catch (Exception ex) when (ex is InvalidOperationException or OperationCanceledException or ObjectDisposedException)
                        {
                            // Conexão encerrada pelo cliente, ignorar
                        }
                    });
            }
            catch (OperationCanceledException)
            {
                // Conexão cancelada pelo cliente
            }
        });
    }
}