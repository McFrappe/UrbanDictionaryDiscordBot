using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

public class Program
{

    private DiscordSocketClient? client;
    private string api_path = "https://api.urbandictionary.com/v0/define?term=";
    public static Task Main(string[] args) => new Program().MainAsync();

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    public async Task<string> FetchDefinitionFromUD(string word)
    {
        using HttpClient client = new();

        HttpResponseMessage response = await client.GetAsync(api_path + word);

        if (!response.IsSuccessStatusCode)
        {
            return $"Error: {response.StatusCode}";
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        return responseBody;
    }
    public async Task MainAsync()
    {
        client = new DiscordSocketClient();
        client.Log += Log;

        var token = JsonConvert.DeserializeObject<ConfigurationClass>(File.ReadAllText("config.json"))?.Token;
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        string response = await FetchDefinitionFromUD("fahad");
        Console.WriteLine(response);


        // Block until the program is closed.
        await Task.Delay(-1);
    }
}