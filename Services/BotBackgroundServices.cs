using Telegram.Bot;
using Telegram.Bot.Polling;

namespace bot.Services;
class BotBackgroundService : BackgroundService
{
    private readonly TelegramBotClient _client;
    private readonly ILogger<BotBackgroundService> _logger;
    private readonly IUpdateHandler _handler;

    public BotBackgroundService(
        ILogger<BotBackgroundService> logger,
        TelegramBotClient client,
        IUpdateHandler handler
    )
    {
        _client=client;
        _logger=logger;
        _handler=handler;
        
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bot=await _client.GetMeAsync(stoppingToken);
        _logger.LogInformation("Bot is working and botusername => {bot.Username} ",bot.Username);
        _client.StartReceiving(
            _handler.HandleUpdateAsync,
            _handler.HandlePollingErrorAsync,
            new ReceiverOptions()
            {
                ThrowPendingUpdates=true
            },
            stoppingToken
        );

    }
}