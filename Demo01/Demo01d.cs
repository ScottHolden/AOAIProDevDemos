using Azure.AI.OpenAI;

class Demo01d
{
    public static async Task RunAsync(OpenAIClient openAIClient, string chatCompletionDeployment)
    {
        string imagePath = "math.jpg";
        string imageContent = "data:image/jpeg;base64," + Convert.ToBase64String(File.ReadAllBytes(imagePath));

        Console.WriteLine("\n---\nDemo01d - Multi-modal");
        Console.Write($"Enter a question to ask of {imagePath}: ");
        var userInput = Console.ReadLine();

        var options = new ChatCompletionsOptions(
            "gpt-4-vision",
            [
                new ChatRequestSystemMessage("Analyze attached images and answer the users question, if the question is not related to the image, return '0' and nothing else."),
                new ChatRequestUserMessage(
                    new ChatMessageTextContentItem(userInput),
                    new ChatMessageImageContentItem(new Uri(imageContent))
                )
            ]
        ){
            MaxTokens = 1000
        };
        var result = await openAIClient.GetChatCompletionsAsync(options);
        var response = result.Value.Choices[0].Message.Content;

        Console.WriteLine($"User Input: {userInput}");
        Console.WriteLine($"Response: {response}");
    }
}