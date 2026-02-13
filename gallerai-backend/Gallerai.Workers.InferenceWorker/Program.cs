using Gallerai.SharedKernel.Settings;
using Gallerai.SignalR.Shared.Consts;
using Gallerai.SignalR.Shared.Hubs;
using Gallerai.Workers.InferenceWorker;
using Gallerai.Workers.InferenceWorker.Consumers;
using Gallerai.Workers.InferenceWorker.Extensions;
using Gallerai.Workers.InferenceWorker.Persistance;
using Gallerai.Workers.InferenceWorker.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddGeneratedSettings(builder.Configuration);

if (Environment.GetEnvironmentVariable("USE_FAKE_INFERENCE") == "true")
{
    builder.Services.AddSingleton<IInferenceService, FakeInferenceService>();
}
else
{
    builder.Services.AddSingleton<IInferenceService, InferenceService>();
}


var rabbitMqSettings = builder.Configuration.GetConfiguration<RabbitMQSettings>();
var redisSettings = builder.Configuration.GetConfiguration<RedisSettings>();
var dbConnection = builder.Configuration.GetConfiguration<DatabaseSettings>().ConnectionString;

builder.Services.AddDbContext<WorkerDbContext>(options =>
            options.UseNpgsql(dbConnection));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StartAIInferenceConsumer>();

    x.AddEntityFrameworkOutbox<WorkerDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);
        o.UsePostgres();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(rabbitMqSettings.Host, "/", h =>
        {
            h.Username(rabbitMqSettings.UserName);
            h.Password(rabbitMqSettings.Password);
        });

        cfg.UseRawJsonSerializer();

        cfg.ConfigureEndpoints(ctx);
    });
});

builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisSettings.ConnectionString, options =>
    {
        options.Configuration.ChannelPrefix = RedisChannel.Literal(ChannelConsts.Gallerai);
    });

builder.Services.AddSingleton<RedisPublisher>();


var host = builder.Build();

host.MapHub<ImageNotificationsHub>(HubConsts.ImagesHub);

await host.UseApplyMigrations();

host.Run();
