using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using dotenv.net;
using Microsoft.Agents.AI;
using OpenAI;

DotEnv.Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");  
var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY") ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY is not set.");  
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

var instructions = """  
                     You are LearnBuddy, a friendly and knowledgeable study coach.                   
                     Your goal is to help the user understand complex topics clearly and progressively. 
                   """;

AIAgent agent = new AzureOpenAIClient(  
        new Uri(endpoint),   
        new AzureKeyCredential(apiKey))  
    .GetChatClient(deploymentName)  
    .CreateAIAgent(instructions: instructions, name: "LearnBuddy");

AgentThread thread = agent.GetNewThread();

var firstQuestion = "Hi my name is Carlos. Could you explain what embeddings are in AI.";

Console.WriteLine(firstQuestion);

await foreach (var update in agent.RunStreamingAsync(firstQuestion,thread))  
{  
    Console.Write(update);  
}

Console.WriteLine();

var secondQuestion = "Whats my name?";
Console.WriteLine(secondQuestion);
await foreach (var update in agent.RunStreamingAsync(secondQuestion, thread))  
{  
    Console.Write(update);  
}

JsonElement serializeThread = thread.Serialize();

string tempFilePath =  Path.GetTempFileName();
File.WriteAllText(tempFilePath, serializeThread.ToString());

JsonElement reloadedSerializedThread = JsonSerializer.Deserialize<JsonElement>(await File.ReadAllTextAsync(tempFilePath));
AgentThread resumedThread = agent.DeserializeThread(reloadedSerializedThread);

Console.WriteLine(await agent.RunAsync("Whats my name?", resumedThread));
