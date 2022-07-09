using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.Services.BotUpdateHandler;

public partial class BotUpdateHandler
{
     private Task HandleMessageAsync(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        var from=message.From;

   var messageType=message.Type switch
        {
            MessageType.Text => HandleTextMessageAsync(botClient,message,cancellationToken),
            _ => HandleUnknownMessageAsync(botClient,message,cancellationToken)
            
            
        };
        
        _logger.LogInformation("Recieved message from {from.Username}",from?.Username);
        return Task.CompletedTask;
    }

    private Task HandleUnknownMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("recieved mesage type {message.Type}", message.Type);
        return Task.CompletedTask;
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var from=message.From;
        _logger.LogInformation("from: {from.FirstName}, message: {message.Text}",from?.FirstName,message.Text);
        await botClient.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text:_localizer["greeting"],
            replyToMessageId:message.MessageId,
            cancellationToken:cancellationToken


        );
    }
}