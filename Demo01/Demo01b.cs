using System.Text.Json;
using Azure.AI.OpenAI;

class Demo01b
{
    public static async Task RunAsync(OpenAIClient openAIClient, string chatCompletionDeployment)
    {
        Console.WriteLine("\n---\nDemo01b - Chat Completion to structured data");
        Console.Write("Enter a topic to generate a short description: ");
        var userInput = Console.ReadLine();

        var options = new ChatCompletionsOptions(
            chatCompletionDeployment,
            [
                new ChatRequestSystemMessage(
                """
                Write a short single sentence description on the given topic.
                Output as a JSON object with the following format:
                {
                    "description": "...",
                    "topics": [ "..." ]
                }
                """),
            new ChatRequestUserMessage(userInput)
            ]
        )
        {
            ResponseFormat = ChatCompletionsResponseFormat.JsonObject,
            Seed = 42,
            MaxTokens = 10
        };
        var result = await openAIClient.GetChatCompletionsAsync(options);
        var response = result.Value.Choices[0].Message.Content;

        Console.WriteLine($"User Input: {userInput}");
        Console.WriteLine($"Raw Response: {response}");
        Console.WriteLine($"Finish reason: {result.Value.Choices[0].FinishReason}");

        var structuredData = JsonSerializer.Deserialize<ShortDescription>(response);
        Console.WriteLine($"Parsed Response: {structuredData}");
        Console.WriteLine($"Is Valid: {structuredData?.IsValid}");
    }
}

record ShortDescription(string? description, string[]? topics)
{
    public bool IsValid => !string.IsNullOrWhiteSpace(description);
}