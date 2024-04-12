using System.ComponentModel;
using Azure.AI.OpenAI;

class Demo01c
{
    public static async Task RunAsync(OpenAIClient openAIClient, string chatCompletionDeployment)
    {
        Console.WriteLine("\n---\nDemo01c - Chat Completion with Tools");
        Console.Write("Enter a username: ");
        var userInput = Console.ReadLine();

        FunctionBuilder functionBuilder = new();

        var options = new ChatCompletionsOptions(
            chatCompletionDeployment,
            [
                new ChatRequestSystemMessage(
                """
                Provide weather details for the given username
                """),
            new ChatRequestUserMessage(userInput)
            ]
        )
        {
            Tools = {
            new ChatCompletionsFunctionToolDefinition(functionBuilder.BuildFunction<WeatherToolRequest>(
                "getWeather",
                "Takes a location and returns the weather",
                request => BusinessService.GetWeather(request.location, request.format)
            )),
            new ChatCompletionsFunctionToolDefinition(functionBuilder.BuildFunction<UserLocationRequest>(
                "getUserLocation",
                "Returns the location of a user by username",
                request => BusinessService.GetUserLocation(request.username)
            )),
        },
            ToolChoice = ChatCompletionsToolChoice.Auto
        };

        while (true)
        {
            var result = await openAIClient.GetChatCompletionsAsync(options);
            var message = result.Value.Choices[0].Message;
            var response = message.Content;
            var toolCalls = message.ToolCalls;

            if (toolCalls != null && toolCalls.Count > 0)
            {
                options.Messages.Add(new ChatRequestAssistantMessage(message));

                foreach (var toolCall in toolCalls.OfType<ChatCompletionsFunctionToolCall>())
                {
                    Console.WriteLine($"Tool Call Id: {toolCall.Id}");
                    Console.WriteLine($"Tool Call Name: {toolCall.Name}");
                    Console.WriteLine($"Tool Call Arguments: {toolCall.Arguments}");

                    //options.Messages.Add(result.Value.Choices[0].Message.)
                    string toolResponse = "";
                    try
                    {
                        toolResponse = await functionBuilder.InvokeFunction(toolCall.Name, toolCall.Arguments);
                    }
                    catch (Exception ex)
                    {
                        toolResponse = $"Error: {ex.Message}";
                    }

                    Console.WriteLine(toolResponse);
                    var toolResponseMessage = new ChatRequestToolMessage(toolResponse, toolCall.Id);
                    options.Messages.Add(toolResponseMessage);
                }
                Console.WriteLine(">- Continuing...");
                continue;
            }


            Console.WriteLine($"User Input: {userInput}");
            Console.WriteLine($"Response: {response}");
            break;
        }
    }
}

record WeatherToolRequest(
    [property: Description("The location to get the weather for")] string location, 
    [property: Description("Either celsius or fahrenheit, defaults to celsius on null")] string? format
);
record UserLocationRequest(
    [property: Description("The username to search for")] string username 
);


class BusinessService
{
    public static Task<string> GetWeather(string location, string? format)
    {
        return Task.FromResult($"{location}: 25 degrees celsius");
    }
    public static Task<string> GetUserLocation(string username)
    {
        return Task.FromResult(username.Equals("scott", StringComparison.OrdinalIgnoreCase) ? $"Melbourne, Australia" : "Error: Unknown user");
    }
}