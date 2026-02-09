using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace UsingMiddlewares.Application;

public class Service(IOptions<AzureAiSettings> options)
{
    private IChatClient GetChatClient()
    {
        var chatClient = new AzureOpenAIClient(
                new Uri(options.Value.Uri),
                new DefaultAzureCredential())
            .GetChatClient(options.Value.Model)
            .AsIChatClient();

        return chatClient;
    }

    private ChatClientAgent CreateChatClientAgent()
    {
        return GetChatClient()
            .AsBuilder()
            .Use(getResponseFunc: new Middlewares().ChatClientMiddleware,
            getStreamingResponseFunc: null)
            .BuildAIAgent(instructions: "You are an AI assistant that helps people find information.",
                tools: [AIFunctionFactory.Create(Tools.GetWeather)]);
    }

    public async Task<string> RunAsync(string userInput)
    {
        var agent = CreateChatClientAgent();
        
        var result = await agent.RunAsync(userInput);
        return result.Text;
    }

    public async Task<string> RunWithWordingGuardrailAsync(string userInput)
    {
        var guardRailAgent = CreateChatClientAgent()
            .AsBuilder()
            .Use(new Middlewares().GuardrailMiddleware, null)
            .Build();

        var result = await guardRailAgent.RunAsync(userInput);
        return result.Text;
    }

    public async Task<string> RunWithFunctionCallingMiddlewareAsync(string userInput)
    {
        var functionMiddlewareAgent = CreateChatClientAgent()
            .AsBuilder()
            .Use(new Middlewares().FunctionCallMiddleware)
            .Build();

        var runOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(Tools.GetDateTime)]
        });
        
        var result = await functionMiddlewareAgent.RunAsync(userInput,options: runOptions);
        return result.Text;
    }
    
    public async Task<string> RunWithFunctionCallingOverrideMiddlewareAsync(string userInput)
    {
        var functionMiddlewareAgent = CreateChatClientAgent().AsBuilder()
            .Use(new Middlewares().FunctionCallOverrideWeather)
            .Build();

        var runOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(Tools.GetDateTime)]
        });
        
        var result = await functionMiddlewareAgent.RunAsync(userInput,options: runOptions);
        return result.Text;
    }
}