using System.Text.Json;
using Azure.AI.OpenAI;
using Json.Schema;
using Json.Schema.Generation;

public class FunctionBuilder
{
    private readonly Dictionary<string, Func<string, Task<string>>> _invokeMap = new(StringComparer.OrdinalIgnoreCase);
	private readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		AllowTrailingCommas = true,
		WriteIndented = false,
		PropertyNameCaseInsensitive = true
	};
	private readonly JsonSchemaBuilder _schemaBuilder = new();
    public FunctionDefinition BuildFunction<T>(string name, string description, Func<T, Task<string>> function)
	{
		if (_invokeMap.ContainsKey(name)) throw new Exception($"Can't have 2 functions added with the name '{name}'");
		_invokeMap.Add(name, async inputJson =>
		{
			var input = JsonSerializer.Deserialize<T>(inputJson, _jsonSerializerOptions);
			if (input == null) return "Error: Invalid function call arguments";
			var output = await function(input);
			return JsonSerializer.Serialize(output, _jsonSerializerOptions);
		});
		var schema = _schemaBuilder.FromType<T>().Build();
		return new FunctionDefinition(name)
		{
			Description = description,
			Parameters = BinaryData.FromObjectAsJson(schema, _jsonSerializerOptions)
		};
	}
    public async Task<string> InvokeFunction(string name, string parameters)
	{
		if (!_invokeMap.TryGetValue(name, out var function)) return $"Unable to find function named {name}";
		return await function(parameters);
	}
}
