﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
//using Newtonsoft.Json;
using System.Text.Json;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    private DiscordSocketClient? Client;
    private CommandService? Commands;
    private string api_path = "https://api.urbandictionary.com/v0/define?term=";

    public async Task HandleCommand(SocketMessage arg)
    {
        int argPos = 0;
        var message = arg as SocketUserMessage;
        var context = new SocketCommandContext(Client, message);

        if (message == null || message.Author.IsBot) return;
        if (!message.HasStringPrefix("!", ref argPos)) return;

        string content = message.Content[1..];
        if (content.Length == 0) return;

        DefinitionsList definitions = await FetchDefinitionFromUD(content);
        if (definitions.list.Count == 0)
        {
            await context.Channel.SendMessageAsync($"There are no definitions for word {content}.");
            return;
        }

        string result = $"Heres the first definition for the word \"{content}\"\n\n{definitions.list[0].definition}";
        await context.Channel.SendMessageAsync(result);
    }

    private static Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

    public async Task<DefinitionsList> FetchDefinitionFromUD(string word)
    {
        using HttpClient client = new();

        HttpResponseMessage response = await client.GetAsync(api_path + word);

        if (!response.IsSuccessStatusCode) return new();

        string responseBody = await response.Content.ReadAsStringAsync();
        if (responseBody == null) return new();

        DefinitionsList definitionsList = JsonSerializer.Deserialize<DefinitionsList>(responseBody);
        if (definitionsList == null) return new();

        return definitionsList;
    }
    public async Task MainAsync()
    {
        Client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent,
            LogLevel = LogSeverity.Info,
        });

        Commands = new CommandService(new CommandServiceConfig
        {
            LogLevel = LogSeverity.Info,
            CaseSensitiveCommands = false,
        });

        Client.Log += Log;
        Client.MessageReceived += HandleCommand;
        Commands.Log += Log;

        var token = JsonSerializer.Deserialize<Configuration>(File.ReadAllText("config.json"))?.Token;
        await Client.LoginAsync(TokenType.Bot, token);
        await Client.StartAsync();

        // Block until the program is closed.
        await Task.Delay(-1);
    }
}