using Azure.AI.OpenAI;
using Azure.Identity;

// Configuration
string aoaiEndpoint = "<endpoint goes here>";
string chatCompletionDeployment = "gpt-4-preview";

// Identity
var identity = new DefaultAzureCredential(new DefaultAzureCredentialOptions { });

// OpenAI Client
var openAIClient = new OpenAIClient(new Uri(aoaiEndpoint), identity);
Console.WriteLine("Warming up...");
await openAIClient.GetChatCompletionsAsync(new ChatCompletionsOptions(chatCompletionDeployment, [new ChatRequestSystemMessage("A")]) { MaxTokens = 1 });

// Run

await Demo01a.RunAsync(openAIClient, chatCompletionDeployment);
await Demo01b.RunAsync(openAIClient, chatCompletionDeployment);
await Demo01c.RunAsync(openAIClient, chatCompletionDeployment);
await Demo01d.RunAsync(openAIClient, chatCompletionDeployment);