using Azure.AI.Agents.Persistent;
using Azure.Identity;
using dotenv.net;
using Microsoft.Agents.AI;

DotEnv.Load();

var endpoint = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");  
var deploymentName = Environment.GetEnvironmentVariable("AZURE_FOUNDRY_PROJECT_DEPLOYMENT_NAME") ?? "gpt-4.1-mini";

var instructions = """  
                     You are LearnBuddy, a friendly and knowledgeable study coach.                   
                     Your goal is to help the user understand complex topics clearly and progressively. 
                   """;

var persistentAgentsClient = new PersistentAgentsClient(endpoint, new AzureCliCredential());

AIAgent agent = await persistentAgentsClient.CreateAIAgentAsync(  
        model: deploymentName,
        name: "LearnBuddy",
        instructions: instructions);

AgentThread thread = agent.GetNewThread();

var firstQuestion = "Hi my name is Carlos - Nosso teste atual. Could you explain what embeddings are in AI.";

Console.WriteLine(firstQuestion);

await foreach (var update in agent.RunStreamingAsync(firstQuestion,thread))  
{  
    Console.Write(update);  
}

