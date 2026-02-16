using System.Text;
using Amazon.Runtime;
using Amazon.S3;
using Gallerai.Application.Features.Images.Consumers;
using Gallerai.Application.Interfaces;
using Gallerai.Infrastructure.Extensions;
using Gallerai.Infrastructure.Persistance;
using Gallerai.Infrastructure.Services;
using Gallerai.SharedKernel.Settings;
using Gallerai.SignalR.Shared.Consts;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
namespace Gallerai.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGeneratedSettings(configuration);

        var dbConnection = configuration.GetConfiguration<DatabaseSettings>().ConnectionString;

        var cloudflareR2Settings = configuration.GetConfiguration<CloudflareR2Settings>();

        var rabbitMqSettings = configuration.GetConfiguration<RabbitMQSettings>();

        var googleAuthSettings = configuration.GetConfiguration<GoogleAuthSettings>();

        var jwtSettings = configuration.GetConfiguration<JwtSettings>();

        var redisSettings = configuration.GetConfiguration<RedisSettings>();

        services.AddDbContext<GalleraiDbContext>(options =>
            options.UseNpgsql(dbConnection));

        services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequireDigit = false;
        })
        .AddEntityFrameworkStores<GalleraiDbContext>()
        .AddSignInManager<SignInManager<IdentityUser>>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(IdentityConstants.ExternalScheme)
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "sub",
                RoleClaimType = "role",
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;

                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(HubConsts.ImagesHub))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        })
        .AddGoogle(options =>
        {
            options.ClientId = googleAuthSettings.ClientId;
            options.ClientSecret = googleAuthSettings.ClientSecret;
            options.SignInScheme = IdentityConstants.ExternalScheme;
            options.CallbackPath = "/signin-google";
            options.CorrelationCookie.SameSite = SameSiteMode.None;
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

        });

        services.AddScoped<IGalleraiDbContext>(provider => provider.GetRequiredService<GalleraiDbContext>());

        services.AddSingleton<IAmazonS3>(_ =>
        {
            var credentials = new BasicAWSCredentials(cloudflareR2Settings.AccessKeyId, cloudflareR2Settings.SecretAccessKey);

            var config = new AmazonS3Config
            {
                ServiceURL = cloudflareR2Settings.Endpoint,
                ForcePathStyle = true
            };

            return new AmazonS3Client(credentials, config);
        });

        services.AddScoped<IImageService, ImageService>();

        services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<GalleraiDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UsePostgres();
                o.UseBusOutbox();
            });

            x.AddConsumer<AIInferenceFinishedEventConsumer>();
            x.AddConsumer<ImageUploadedEventConsumer>();

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

        services.AddSignalR()
            .AddStackExchangeRedis(redisSettings.ConnectionString, options =>
            {
                options.Configuration.ChannelPrefix = RedisChannel.Literal(ChannelConsts.Gallerai);
            });

        services.AddSingleton<IConnectionMultiplexer>(config =>
        {
            return ConnectionMultiplexer.Connect(redisSettings.ConnectionString);
        });

        services.AddSingleton<ICacheService, RedisCacheService>();

        services.AddScoped<INotificationService, SignalRNotificationService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
