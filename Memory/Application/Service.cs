using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using OpenAI.Chat;

namespace Memory.Application;

public class Service(IOptions<AzureAiSettings> options)
{
    private static readonly VectorStore VectorStore = new InMemoryVectorStore();

    private ChatClient GetChatClient()
    {
        var chatClient = new AzureOpenAIClient(
                new Uri(options.Value.Uri),
                new DefaultAzureCredential())
            .GetChatClient(options.Value.Model);

        return chatClient;
    }

    public async Task<RunResult> RunWithoutChatHistoryProvider(string userInput, JsonElement? currentState)
    {
        var agent = GetChatClient()
            .AsAIAgent(new ChatClientAgentOptions()
            {
                ChatOptions = new()
                {
                    Instructions = "You are a AI Personal Assistant",
                },
                Name = "AI Personal Assistant"
            });

        AgentSession session;
        if (currentState == null)
        {
            session = await agent.CreateSessionAsync();
        }
        else
        {
            session = await agent.DeserializeSessionAsync(currentState.Value);
        }

        var response = await agent.RunAsync(userInput, session);

        var serializedState = agent.SerializeSession(session);
        Console.WriteLine("\n--- Serialized session ---\n");
        Console.WriteLine(JsonSerializer.Serialize(serializedState,
            new JsonSerializerOptions { WriteIndented = true }));

        return new RunResult(response.Text, serializedState);
    }

    public async Task<RunResult> RunAsync(string userInput, JsonElement? currentState)
    {
        var agent = GetChatClient()
            .AsAIAgent(new ChatClientAgentOptions()
            {
                ChatOptions = new()
                {
                    Instructions = "You are a AI Personal Assistant",
                },
                Name = "AI Personal Assistant",

                ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
                    // Create a new ChatHistoryProvider for this agent that stores chat history in a vector store.
                    // Each session must get its own copy of the VectorChatHistoryProvider, since the provider
                    // also contains the id that the chat history is stored under.
                    new VectorChatHistoryProvider(VectorStore, ctx.SerializedState, ctx.JsonSerializerOptions))
            });

        AgentSession session;
        if (currentState == null)
        {
            session = await agent.CreateSessionAsync();
        }
        else
        {
            session = await agent.DeserializeSessionAsync(currentState.Value);
        }

        var response = await agent.RunAsync(userInput, session);

        var serializedState = agent.SerializeSession(session);
        Console.WriteLine("\n--- Serialized session ---\n");
        Console.WriteLine(JsonSerializer.Serialize(serializedState,
            new JsonSerializerOptions { WriteIndented = true }));

        var chatHistoryProvider = session.GetService<VectorChatHistoryProvider>()!;
        Console.WriteLine($"\nSession is stored in vector store under key: {chatHistoryProvider.SessionDbKey}");

        return new RunResult(response.Text, serializedState);
    }

    public async Task<RunResult> RunAiContextProviderAsync(string userInput, JsonElement? currentState)
    {
        ChatClient chatClient = GetChatClient();

        var agent = GetChatClient()
            .AsAIAgent(new ChatClientAgentOptions()
            {
                ChatOptions = new()
                {
                    Instructions = "You are a friendly assistant. Always address the user by their name.",
                },
                Name = "AI Personal Assistant",
                AIContextProviderFactory = (ctx, ct) =>
                    new ValueTask<AIContextProvider>(new UserInfoMemory(chatClient.AsIChatClient(), ctx.SerializedState,
                        ctx.JsonSerializerOptions))
            });

        AgentSession session;
        if (currentState == null)
        {
            session = await agent.CreateSessionAsync();
        }
        else
        {
            session = await agent.DeserializeSessionAsync(currentState.Value);
        }
        
        var response = await agent.RunAsync(userInput, session);

        var serializedState = agent.SerializeSession(session);
        Console.WriteLine("\n--- Serialized session ---\n");
        Console.WriteLine(JsonSerializer.Serialize(serializedState,
            new JsonSerializerOptions { WriteIndented = true }));
        
        var userInfo = session.GetService<UserInfoMemory>()?.UserInfo;

        // Output the user info that was captured by the memory component.
        Console.WriteLine($"MEMORY - User Name: {userInfo?.UserName}");
        Console.WriteLine($"MEMORY - User Age: {userInfo?.UserAge}");
        
        var newSession = await agent.CreateSessionAsync();
        if (userInfo is not null && newSession.GetService<UserInfoMemory>() is UserInfoMemory newSessionMemory)
        {
            newSessionMemory.UserInfo = userInfo;
        }
        
        return new RunResult(response.Text, serializedState);
    }
    
    
    public async Task<RunResult> RunWithBothAsync(string userInput, JsonElement? currentState)
    {
        ChatClient chatClient = GetChatClient();

        var agent = GetChatClient()
            .AsAIAgent(new ChatClientAgentOptions()
            {
                ChatOptions = new()
                {
                    Instructions = "You are a friendly assistant. Always address the user by their name.",
                },
                Name = "AI Personal Assistant",
                ChatHistoryProviderFactory = (ctx, ct) => new ValueTask<ChatHistoryProvider>(
                    // Create a new ChatHistoryProvider for this agent that stores chat history in a vector store.
                    // Each session must get its own copy of the VectorChatHistoryProvider, since the provider
                    // also contains the id that the chat history is stored under.
                    new VectorChatHistoryProvider(VectorStore, ctx.SerializedState, ctx.JsonSerializerOptions)),
                AIContextProviderFactory = (ctx, ct) =>
                    new ValueTask<AIContextProvider>(new UserInfoMemory(chatClient.AsIChatClient(), ctx.SerializedState,
                        ctx.JsonSerializerOptions))
            });

        AgentSession session;
        if (currentState == null)
        {
            session = await agent.CreateSessionAsync();
        }
        else
        {
            session = await agent.DeserializeSessionAsync(currentState.Value);
        }
        
        var response = await agent.RunAsync(userInput, session);

        var serializedState = agent.SerializeSession(session);
        Console.WriteLine("\n--- Serialized session ---\n");
        Console.WriteLine(JsonSerializer.Serialize(serializedState,
            new JsonSerializerOptions { WriteIndented = true }));
        
        var userInfo = session.GetService<UserInfoMemory>()?.UserInfo;

        // Output the user info that was captured by the memory component.
        Console.WriteLine($"MEMORY - User Name: {userInfo?.UserName}");
        Console.WriteLine($"MEMORY - User Age: {userInfo?.UserAge}");
        
        var newSession = await agent.CreateSessionAsync();
        if (userInfo is not null && newSession.GetService<UserInfoMemory>() is UserInfoMemory newSessionMemory)
        {
            newSessionMemory.UserInfo = userInfo;
        }
        
        return new RunResult(response.Text, serializedState);
    }

}

