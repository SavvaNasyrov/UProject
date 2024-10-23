using Telegram.Bot;
using UProject.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Secrets/token.json");

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddScoped<ITelegramBotClient, TelegramBotClient>(x =>
{
    var config = x.GetRequiredService<IConfiguration>();

    var token = config["BotToken"] ?? throw new NullReferenceException("Token was null");
    var webhookaddress = config["BotWebHook"] ?? throw new NullReferenceException("BotWebHook was null");

    var bot = new TelegramBotClient(new TelegramBotClientOptions(token));

    bot.SetWebhookAsync(webhookaddress).Wait();

    return bot;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
