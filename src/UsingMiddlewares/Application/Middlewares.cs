using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace UsingMiddlewares.Application;

public class Middlewares
{
    // This middleware handles chat client lower level invocations.
    // This is useful for handling agent messages before they are sent to the LLM and also handle any response messages from the LLM before they are sent back to the agent.
    public async Task<ChatResponse> ChatClientMiddleware(IEnumerable<ChatMessage> message, ChatOptions? options, IChatClient innerChatClient, CancellationToken cancellationToken)
    {
        Console.WriteLine("Chat Client Middleware - Pre-Chat");
        var response = await innerChatClient.GetResponseAsync(message, options, cancellationToken);
        Console.WriteLine("Chat Client Middleware - Post-Chat");

        return response;
    }
    
    // This middleware enforces guardrails by redacting certain keywords from input and output messages.
    public async Task<AgentResponse> GuardrailMiddleware(IEnumerable<ChatMessage> messages, AgentSession? session, AgentRunOptions? options, AIAgent innerAgent, CancellationToken cancellationToken)
    {
        // Redact keywords from input messages
        var filteredMessages = FilterMessages(messages);

        Console.WriteLine("Guardrail Middleware - Filtered messages Pre-Run");

        // Proceed with the agent run
        var response = await innerAgent.RunAsync(filteredMessages, session, options, cancellationToken);

        // Redact keywords from output messages
        response.Messages = FilterMessages(response.Messages);

        Console.WriteLine("Guardrail Middleware - Filtered messages Post-Run");

        return response;

        List<ChatMessage> FilterMessages(IEnumerable<ChatMessage> messages)
        {
            return messages.Select(m => new ChatMessage(m.Role, FilterContent(m.Text))).ToList();
        }

        static string FilterContent(string content)
        {
            foreach (var keyword in new[] { "ofensivo", "ilegal", "violento" })
            {
                if (content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return "[CENSURADO: Conteúdo proibido]";
                }
            }

            return content;
        }
    }

    public async ValueTask<object?> FunctionCallMiddleware(AIAgent agent, FunctionInvocationContext context,
        Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Function Name: {context!.Function.Name} - Middleware 1 Pre-Invoke");
        var result = await next(context, cancellationToken);
        Console.WriteLine($"Function Name: {context!.Function.Name} - Middleware 1 Post-Invoke");

        return result;
    }
    
    // Function invocation middleware that overrides the result of the GetWeather function.
    public async ValueTask<object?> FunctionCallOverrideWeather(AIAgent agent, FunctionInvocationContext context, Func<FunctionInvocationContext, CancellationToken, ValueTask<object?>> next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Function Name: {context!.Function.Name} - Middleware 2 Pre-Invoke");

        var result = await next(context, cancellationToken);

        if (context.Function.Name == nameof(Tools.GetWeather))
        {
            // Override the result of the GetWeather function
            result = "The weather is sunny with a high of 25°C.";
        }
        Console.WriteLine($"Function Name: {context!.Function.Name} - Middleware 2 Post-Invoke");
        return result;
    }
}