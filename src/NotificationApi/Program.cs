using NotificationApi.Interfaces;
using NotificationApi.Options;
using NotificationApi.RateLimiting;
using NotificationApi.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<OpenAIOptions>(
    builder.Configuration.GetSection("OpenAI"));

builder.Services.Configure<DiscordOptions>(
    builder.Configuration.GetSection("Discord"));


builder.Services.AddSingleton<IRateLimiter, InMemoryRateLimiter>();
builder.Services.AddScoped<INotificationProcessor, NotificationProcessor>();

builder.Services.AddHttpClient<ILLMService, OpenAIService>(client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/v1/");
    client.DefaultRequestHeaders.Add(
        "Authorization",
        $"Bearer {builder.Configuration["OpenAI:ApiKey"]}"
    );
});

builder.Services.AddHttpClient<IDiscordNotifier, DiscordNotifier>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();

public partial class Program { }