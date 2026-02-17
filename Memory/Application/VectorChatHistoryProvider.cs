using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace Memory.Application;

/// <summary>
/// A sample implementation of <see cref="ChatHistoryProvider"/> that stores chat history in a vector store.
/// </summary>
public sealed class VectorChatHistoryProvider : ChatHistoryProvider
{
    private readonly VectorStore _vectorStore;

    public VectorChatHistoryProvider(
        VectorStore vectorStore, 
        JsonElement serializedState, 
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _vectorStore = vectorStore ?? throw new ArgumentNullException(nameof(vectorStore));

        if (serializedState.ValueKind is JsonValueKind.String)
        {
            SessionDbKey = serializedState.Deserialize<string>();
        }
    }

    public string? SessionDbKey { get; set; }

    protected override async ValueTask<IEnumerable<ChatMessage>> InvokingCoreAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        var collection = this._vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        var records = await collection
            .GetAsync(
                x => x.SessionId == this.SessionDbKey,
                10, new() { OrderBy = x => x.Descending(y => y.Timestamp) },
                cancellationToken)
            .ToListAsync(cancellationToken);

        var messages = records.ConvertAll(x => JsonSerializer.Deserialize<ChatMessage>(x.SerializedMessage!)!)
;
        messages.Reverse();
        return messages;
    }

    protected override async ValueTask InvokedCoreAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        // Don't store messages if the request failed.
        if (context.InvokeException is not null)
        {
            return;
        }

        SessionDbKey ??= Guid.NewGuid().ToString("N");

        var collection = _vectorStore.GetCollection<string, ChatHistoryItem>("ChatHistory");
        await collection.EnsureCollectionExistsAsync(cancellationToken);

        // Add both request and response messages to the store
        // Optionally messages produced by the AIContextProvider can also be persisted (not shown).
        var allNewMessages = context.RequestMessages.Concat(context.ResponseMessages ?? []);

        await collection.UpsertAsync(allNewMessages.Select(x => new ChatHistoryItem()
        {
            Key = $"{SessionDbKey}_{x.MessageId}_{Guid.NewGuid()}",
            Timestamp = DateTimeOffset.UtcNow,
            SessionId = SessionDbKey,
            SerializedMessage = JsonSerializer.Serialize(x),
            MessageText = x.Text
        }), cancellationToken);
    }

    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null) =>
        // We have to serialize the session id, so that on deserialization we can retrieve the messages using the same session id.
        JsonSerializer.SerializeToElement(SessionDbKey);

    /// <summary>
    /// The data structure used to store chat history items in the vector store.
    /// </summary>
    private sealed class ChatHistoryItem
    {
        [VectorStoreKey]
        public string? Key { get; set; }

        [VectorStoreData]
        public string? SessionId { get; set; }

        [VectorStoreData]
        public DateTimeOffset? Timestamp { get; set; }

        [VectorStoreData]
        public string? SerializedMessage { get; set; }

        [VectorStoreData]
        public string? MessageText { get; set; }
    }
}