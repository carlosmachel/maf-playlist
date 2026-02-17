using System.Text;
using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace Memory.Application;

 /// <summary>
/// Sample memory component that can remember a user's name and age.
/// </summary>
public sealed class UserInfoMemory : AIContextProvider
{
    private readonly IChatClient _chatClient;

    public UserInfoMemory(
        IChatClient chatClient, 
        UserInfo? userInfo = null)
    {
        _chatClient = chatClient;
        UserInfo = userInfo ?? new UserInfo();
    }

    public UserInfoMemory(
        IChatClient chatClient, 
        JsonElement serializedState, 
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _chatClient = chatClient;

        UserInfo = serializedState.ValueKind == JsonValueKind.Object ?
            serializedState.Deserialize<UserInfo>(jsonSerializerOptions)! :
            new UserInfo();
    }

    public UserInfo UserInfo { get; set; }

    protected override async ValueTask InvokedCoreAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        // Try and extract the user name and age from the message if we don't have it already and it's a user message.
        if ((UserInfo.UserName is null || UserInfo.UserAge is null) && context.RequestMessages.Any(x => x.Role == ChatRole.User))
        {
            var result = await _chatClient.GetResponseAsync<UserInfo>(
                context.RequestMessages,
                new ChatOptions
                {
                    Instructions = "Extract the user's name and age from the message if present. If not present return nulls."
                },
                cancellationToken: cancellationToken);

            UserInfo.UserName ??= result.Result.UserName;
            UserInfo.UserAge ??= result.Result.UserAge;
        }
    }

    protected override ValueTask<AIContext> InvokingCoreAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        StringBuilder instructions = new();

        // If we don't already know the user's name and age, add instructions to ask for them, otherwise just provide what we have to the context.
        instructions
            .AppendLine(
                UserInfo.UserName is null ?
                    "Ask the user for their name and politely decline to answer any questions until they provide it." :
                    $"The user's name is {this.UserInfo.UserName}.")
            .AppendLine(
                UserInfo.UserAge is null ?
                    "Ask the user for their age and politely decline to answer any questions until they provide it." :
                    $"The user's age is {this.UserInfo.UserAge}.");

        return new ValueTask<AIContext>(new AIContext
        {
            Instructions = instructions.ToString()
        });
    }

    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return JsonSerializer.SerializeToElement(this.UserInfo, jsonSerializerOptions);
    }
}

