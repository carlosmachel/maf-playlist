using System.Text.Json;

namespace Memory.Application;

public record RunResult(string Text, JsonElement SerializedState);

public sealed class UserInfo
{
    public string? UserName { get; set; }
    public int? UserAge { get; set; }
}