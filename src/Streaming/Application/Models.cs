using System.Text.Json.Serialization;

namespace Streaming.Application;


// Tipos de resultado para streaming
[JsonDerivedType(typeof(RunMessageUpdateResult))]
[JsonDerivedType(typeof(TokenUsageResult))]
public abstract record RunUpdateResult();

/// <summary>
/// Resultado de uma atualização de mensagem durante streaming
/// </summary>
public record RunMessageUpdateResult(string Id, string Text, bool IsComplete) : RunUpdateResult;

/// <summary>
/// Resultado com dados de uso de tokens para monitoramento de custos
/// </summary>
public record TokenUsageResult(
    string MessageId,
    long InputTokens,
    long OutputTokens,
    long TotalTokens
) : RunUpdateResult;

public record RunMessageResult(
    string MessageId, 
    string Text, 
    long InputTokens, 
    long OutputTokens, 
    long TotalTokens);
