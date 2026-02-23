using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Streaming.Application;

public class Service(IOptions<AzureAiSettings> options)
{
    private ChatClient GetChatClient()
    {
        var chatClient = new AzureOpenAIClient(
                new Uri(options.Value.Uri),
                new DefaultAzureCredential())
            .GetChatClient(options.Value.Model);

        return chatClient;
    }
    
    private ChatClientAgent CreateChatClientAgent()
    {
        return GetChatClient()
            .AsAIAgent(instructions: "You are an AI assistant that helps people find information.",
                tools: [AIFunctionFactory.Create(Tools.GetWeather)]);
    }

    public async Task<RunMessageResult> RunAsync(string userInput)
    {
        var agent = CreateChatClientAgent();
        var result = await agent.RunAsync(userInput);
        return new RunMessageResult(
            result.ResponseId!, 
            result.Text, 
            result.Usage!.InputTokenCount.GetValueOrDefault(),
            result.Usage.OutputTokenCount.GetValueOrDefault(), 
            result.Usage.TotalTokenCount.GetValueOrDefault());
    }
    
    public async Task<RunMessageResult> RunToAgentResponseAsync(string userInput)
    {
        var agent = CreateChatClientAgent();
        var result = await agent.RunStreamingAsync(userInput)
            .ToAgentResponseAsync();
        
        return new RunMessageResult(
            result.ResponseId!, 
            result.Text, 
            result.Usage!.InputTokenCount.GetValueOrDefault(),
            result.Usage.OutputTokenCount.GetValueOrDefault(), 
            result.Usage.TotalTokenCount.GetValueOrDefault());
    }
    
    public async Task RunStreamingAsync(string userInput, Func<RunUpdateResult, Task> onUpdate)
    {
        var agent = CreateChatClientAgent();
        var updates = agent.RunStreamingAsync(userInput);

        await foreach (var update in updates)
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                await onUpdate(new RunMessageUpdateResult(
                    update.MessageId!, 
                    update.Text,
                    IsComplete: false
                ));
            }
            
            foreach (var content in update.Contents)
            {
                if (content is UsageContent usageContent)
                {
                    var tokenUsage = new TokenUsageResult(
                        MessageId: update.MessageId!,
                        InputTokens: usageContent.Details.InputTokenCount ?? 0,
                        OutputTokens: usageContent.Details.OutputTokenCount ?? 0,
                        TotalTokens: usageContent.Details.TotalTokenCount ?? 0
                    );
                    
                    await onUpdate(tokenUsage);
                    
                    await onUpdate(new RunMessageUpdateResult(
                        update.MessageId!,
                        string.Empty, 
                        IsComplete: true
                    ));
                }
            }
        }
    }
}
