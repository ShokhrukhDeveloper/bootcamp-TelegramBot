using bot.Resources;
using Microsoft.Extensions.Localization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace bot.Services.BotUpdateHandler;

public partial class BotUpdateHandler : IUpdateHandler
{
    private readonly ILogger<BotUpdateHandler> _logger;
    private readonly TelegramBotClient _client;
    private readonly IServiceScopeFactory _scopeFactory;
    private  IStringLocalizer<BotLocalizer> _localizer;

    public BotUpdateHandler(
        ILogger<BotUpdateHandler> logger,
        TelegramBotClient client,
        IServiceScopeFactory scopeFactory
    )
    {
        _logger=logger;
        _client=client;
        _scopeFactory=scopeFactory;
        
    }
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        using var scope=_scopeFactory.CreateScope();
        _localizer=scope.ServiceProvider.GetRequiredService<IStringLocalizer<BotLocalizer>>();
        _logger.LogInformation("recievde message={update.Message.Text}",update?.Message?.Text);
       var handler = update?.Type switch
       {
            UpdateType.Message => HandleMessageAsync(botClient,update.Message,cancellationToken),
            UpdateType.EditedMessage => HandleEditMessageAsync(botClient,update.EditedMessage,cancellationToken),
            UpdateType.CallbackQuery => HandleMessageCallbackQueryAsync(botClient,update.Message,cancellationToken),
            UpdateType.ChannelPost => HandleChannelPostCallbackQueryAsync(botClient,update.ChannelPost,cancellationToken),
            UpdateType.ChatJoinRequest=>HandleChatJoinRequestAsync(botClient,update.ChatJoinRequest,cancellationToken),
            UpdateType.ChatMember =>HandleChatMemberAsync(botClient,update.ChatMember,cancellationToken),
            UpdateType.Poll =>HandlePollAsync(botClient,update.Poll,cancellationToken),
            UpdateType.PollAnswer => HandlePollAnswer(botClient,update.PollAnswer,cancellationToken),
            





            UpdateType.Unknown or _ => UnknownMessageHandler(botClient , update?.Message,cancellationToken) 

       };
       try
       {
          await handler;     
       }
       catch (System.Exception e)
       {
        
        
        await HandlePollingErrorAsync(botClient,e,cancellationToken);
       }

    }

  

    private Task UnknownMessageHandler(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {   
        throw new NotImplementedException();
    }
}