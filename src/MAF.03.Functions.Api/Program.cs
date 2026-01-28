using System.ComponentModel;
using Azure.AI.OpenAI;
using Azure.Identity;
using MAF._03.Functions.Api.Models;
using Microsoft.Agents.AI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using Scalar.AspNetCore;
using ChatResponse = MAF._03.Functions.Api.Models.ChatResponse;

#pragma warning disable MEAI001

var builder = WebApplication.CreateBuilder(args);

// OpenAPI / Scalar
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

// Scalar UI
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("Agent API") 
        .WithTheme(ScalarTheme.Default)
        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

var endpoint = builder.Configuration["Settings:Endpoint"]
              ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");

var deploymentName = builder.Configuration["Settings:Model"]
                     ?? "gpt-4.1-mini";

[Description("Get the weather for a given location.")]
static string GetWeather(
    [Description("The location to get the weather for.")] string location)
{
    return $"The weather in {location} is cloudy with a high of 15°C.";
}

AIAgent CreateAgent(bool requireApproval)
{
    var chatClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new DefaultAzureCredential())
        .GetChatClient(deploymentName);

    var tool = AIFunctionFactory.Create(GetWeather);

    if (requireApproval)
    {
        tool = new ApprovalRequiredAIFunction(tool);
    }

    return chatClient
        .AsIChatClient().AsAIAgent(
        instructions: "You are a helpful assistant.",
        tools: [tool]
    );
}

var threads = new Dictionary<string, AgentSession>();

app.MapPost("/api/chat", async (
    [FromBody] ChatRequest request) =>
{
    try
    {
        var agent = CreateAgent(request.RequireApproval);
        
        AgentSession thread;
        string threadId;

        if (!string.IsNullOrWhiteSpace(request.ThreadId) &&
            threads.TryGetValue(request.ThreadId, out var existingThread))
        {
            thread = existingThread;
            threadId = request.ThreadId!;
        }
        else
        {
            thread = await agent.GetNewSessionAsync();
            threadId = Guid.NewGuid().ToString();
            threads[threadId] = thread;
        }
        
        var response = await agent.RunAsync(request.Message, thread);
        
        var approvals = response.UserInputRequests
            .OfType<FunctionApprovalRequestContent>()
            .Select(x => new PendingFunctionApproval(
                x.Id,
                x.FunctionCall.CallId,
                x.FunctionCall.Name,
                x.FunctionCall.Arguments
            ))
            .ToList();

        if (approvals.Count > 0)
        {
            return Results.Ok(new ChatResponse(
                Success: true,
                Message: null,
                ThreadId: threadId,
                PendingApprovals: approvals
            ));
        }

        return Results.Ok(new ChatResponse(
            Success: true,
            Message: response.ToString(),
            ThreadId: threadId,
            PendingApprovals: null
        ));
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/chat/approve", async (
    [FromBody] ApprovalResponse request) =>
{
    if (!threads.TryGetValue(request.ThreadId, out var thread))
    {
        return Results.NotFound("Thread not found.");
    }

    var agent = CreateAgent(requireApproval: true);

    var userMessage = new ChatMessage(
        ChatRole.User,
        [
            new FunctionApprovalResponseContent(request.ApprovalId, request.Approved, new FunctionCallContent(request.CallId, request.FunctionName, request.ArgumentsJson))
        ]);

    var response = await agent.RunAsync([userMessage], thread);

    return Results.Ok(new ChatResponse(
        Success: true,
        Message: response.ToString(),
        ThreadId: request.ThreadId,
        PendingApprovals: null
    ));
});

app.Run();