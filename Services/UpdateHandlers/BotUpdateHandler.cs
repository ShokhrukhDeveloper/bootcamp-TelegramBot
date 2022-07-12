using System.Globalization;
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
    private UserService _userService;

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
        ///localization

        using var scope=_scopeFactory.CreateScope();       
        _userService=scope.ServiceProvider.GetRequiredService<UserService>();
        var culture= await GetCultureForUserAsync(update);
        CultureInfo.CurrentCulture=culture;
        CultureInfo.CurrentUICulture=culture;
        _localizer=scope.ServiceProvider.GetRequiredService<IStringLocalizer<BotLocalizer>>();                  
        /// set langauge
       

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

    private async Task<CultureInfo> GetCultureForUserAsync(Update update)

    {
        ArgumentNullException.ThrowIfNull(update); 

       User? from=update.Type switch 
       {
            UpdateType.Message=>update.Message?.From,
            UpdateType.EditedMessage => update.EditedMessage?.From,
            UpdateType.CallbackQuery => update.CallbackQuery?.From,
            UpdateType.InlineQuery => update.InlineQuery?.From,
            _ =>update.Message?.From
        };
      var result=  await _userService.AddUserAsync(new Entity.User(){
           FirstName=from.FirstName,
           LastName=from.LastName,
           UserId=from.Id,
           ChatId=update.Message.Chat.Id,
           Isbot=from.IsBot,
           Username=from.Username,
           LanguageCode=from.LanguageCode,
           CreatedAt=DateTimeOffset.UtcNow,
           LastInteractionAt=DateTimeOffset.UtcNow    
        });

        if (result.IsSuccess)
        {
            _logger.LogInformation("New USER ADDED : {from.Id}",from.Id);
        }
        else{
            _logger.LogInformation("New USER NOT ADDED : {from.Id}  {result.ErrorMEssage}",from.Id,result.ErrorMEssage);
        }
        var languageCode= await _userService.GetUserLanguageCodeAsync(from?.Id);
        _logger.LogInformation($"Langage chaned to {languageCode}");
        return new CultureInfo(languageCode??"uz-Uz");


        

    }

    private Task UnknownMessageHandler(ITelegramBotClient botClient, Message? message, CancellationToken cancellationToken)
    {   
        throw new NotImplementedException();
    }
}
