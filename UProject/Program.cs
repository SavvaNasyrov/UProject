using System.Collections.Concurrent;
using Telegram.Bot;
using UProject.Abstractions;
using UProject.Models;
using UProject.Services;
using UProject.Services.NotificationServices;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Secrets/token.json");

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddScoped<IWeatherForecast, WeatherForecast>();

builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(x =>
{
    var config = x.GetRequiredService<IConfiguration>();

    var token = config["BotToken"] ?? throw new NullReferenceException("Token was null");
    var webhookaddress = config["BotWebHook"] ?? throw new NullReferenceException("BotWebHook was null");

    var bot = new TelegramBotClient(new TelegramBotClientOptions(token));

    bot.SetWebhookAsync(webhookaddress).Wait();

    return bot;
});

builder.Services.AddSingleton<ConcurrentQueue<BotMessage>>();

builder.Services.AddHostedService<Sender>();

builder.Services.AddHostedService<OneMinuteNotifyer>();

builder.Services.AddHostedService<OneDayNotifyer>();

builder.Services.AddHostedService<OneWeekNotifyer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
