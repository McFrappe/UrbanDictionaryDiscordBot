using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private DiscordSocketClient? client;
    public async Task MainAsync()
    {
        client = new DiscordSocketClient();
        client.Log += Log;

        var token = JsonConvert.DeserializeObject<ConfigurationClass>(File.ReadAllText("config.json"))?.Token;
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        // Block until the program is closed.
        await Task.Delay(-1);
    }
}