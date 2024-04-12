using Azure.AI.OpenAI;

class Demo01a
{
    public static async Task RunAsync(OpenAIClient openAIClient, string chatCompletionDeployment)
    {
        Console.WriteLine("\n---\nDemo01a - Chat Completion");
        Console.Write("Enter a topic to generate a short description: ");
        var userInput = Console.ReadLine();

        var options = new ChatCompletionsOptions(
            chatCompletionDeployment,
            [
                new ChatRequestSystemMessage("Write a short single sentence description on the given topic"),
                new ChatRequestUserMessage(userInput)
            ]
        );
        var result = await openAIClient.GetChatCompletionsAsync(options);
        var response = result.Value.Choices[0].Message.Content;

        Console.WriteLine($"User Input: {userInput}");
        Console.WriteLine($"Response: {response}");
    }
}