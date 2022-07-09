using Telegram.Bot;
using bot.Services;
using Telegram.Bot.Polling;
using bot.Services.BotUpdateHandler;

var builder = WebApplication.CreateBuilder(args);
var token=builder.Configuration.GetValue<string>("Token");
builder.Services.AddSingleton<TelegramBotClient>(new TelegramBotClient(token));
builder.Services.AddHostedService<BotBackgroundService>();
builder.Services.AddSingleton<IUpdateHandler,BotUpdateHandler>();
builder.Services.AddLocalization();

//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var app = builder.Build();
var supportedCultures = new[] { "uz-Uz","en-Us","ru-Ru" };
var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);




app.Run();
