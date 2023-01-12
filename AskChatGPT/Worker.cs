namespace AskChatGPT;

public class Worker
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient;

    public Worker(ILogger<Worker> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task ExecuteAsync(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("ChatGptApiKey");

        if (apiKey == null)
        {
            throw new ArgumentNullException(nameof(apiKey), "Missing environment variable 'ChatGptApiKey'");
        }

        if (args.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Usage: ask 'some question'");
            Console.ResetColor();
            return;
        }
        try
        {
            var body = new
            {
                model = "text-davinci-001",
                prompt = $"{args[0]}",
                temperature = 1,
                max_tokens = 100
            };

            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            var httpResponse = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/completions", body);
            var stringContent = await httpResponse.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<Response>(stringContent);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(response.Choices.FirstOrDefault().Text);
            Console.ResetColor();

            TextCopy.ClipboardService.SetText(response.Choices.FirstOrDefault().Text);
        }
        catch (Exception ex)
        {
            _logger.LogError("{ex}", ex.Message);
        }
    }
}
