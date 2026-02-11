using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace StructuredOutput.Application;

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

    private ChatClientAgent CreateChatClientAgent(
        string instructions = "You are an AI assistant")
    {
        return GetChatClient()
            .AsBuilder()
            .BuildAIAgent(instructions: instructions);
    }

    public async Task<string> RunWithImageAsync(
        string userInput, 
        Stream fileStream)
    {
        var agent = CreateChatClientAgent();
        
        ChatMessage message = new(ChatRole.User, [
            new TextContent(userInput), 
            new DataContent(
                await BinaryData.FromStreamAsync(fileStream),
                mediaType: "image/png"
            )]);

        var result = await agent.RunAsync(message);
        return result.Text;
    }
    
    public async Task<string> RunWithImageUriAsync(
        string userInput, 
        string url, 
        string mediaType)
    {
        var agent = CreateChatClientAgent();
        
        ChatMessage message = new(ChatRole.User, [
            new TextContent(userInput), 
            new UriContent(url, mediaType)]);

        var result = await agent.RunAsync(message);
        return result.Text;
    }
    
    public async Task<OrdemServicoDto> RunWithStructuredOutput(
        string userInput, 
        Stream fileStream)
    {
        var agent = CreateChatClientAgent();
        
        ChatMessage message = new(ChatRole.User, [
            new TextContent(userInput), 
            new DataContent(
                await BinaryData.FromStreamAsync(fileStream),
                mediaType: "image/png"
            )]);

        AgentResponse<OrdemServicoDto> result = await agent.RunAsync<OrdemServicoDto>(message);
        return result.Result;
    }
    
    public async Task<OrdemServicoDto> RunWithStructuredOutputStreaming(
        string userInput, 
        Stream fileStream)
    {
        var agent = GetChatClient()
            .AsAIAgent(new ChatClientAgentOptions()
        {
            Name = "HelpfulAssistant",
            ChatOptions = new ChatOptions
            {
                Instructions = "You are a helpful assistant",
                ResponseFormat = ChatResponseFormat.ForJsonSchema<OrdemServicoDto>()
            }
        });
        
        ChatMessage message = new(ChatRole.User, [
            new TextContent(userInput), 
            new DataContent(
                await BinaryData.FromStreamAsync(fileStream),
                mediaType: "image/png"
            )]);

        var updates = agent.RunStreamingAsync(message);

        var dto =
            (await updates.ToAgentResponseAsync()).Deserialize<OrdemServicoDto>(JsonSerializerOptions.Web);
        return dto;
    }
}