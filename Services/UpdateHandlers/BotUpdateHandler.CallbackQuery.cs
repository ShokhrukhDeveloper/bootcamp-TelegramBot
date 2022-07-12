using Telegram.Bot;
using Telegram.Bot.Types;

namespace bot.Services.BotUpdateHandler;

public partial class BotUpdateHandler
{
     private Task HandleMessageCallbackQueryAsync(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        var from=message.From;
        
        _logger.LogInformation("Recieved message from {from.Username}",from?.Username);
        return Task.CompletedTask;
    }
    
}