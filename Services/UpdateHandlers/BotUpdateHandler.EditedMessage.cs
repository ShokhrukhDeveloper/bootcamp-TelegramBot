using Telegram.Bot;
using Telegram.Bot.Types;

namespace bot.Services.BotUpdateHandler;

public partial class BotUpdateHandler
{
     private Task HandleEditMessageAsync(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        var from=message.From;
        
        _logger.LogInformation("Recieved EditedMessage from {from.Username} edited {message.Text}",from?.Username);
        return Task.CompletedTask;
    }
    
}