using Amazon.Runtime;
using Amazon.S3;
using Gallerai.Application.Interfaces;
using Gallerai.Infrastructure.Extensions;
using Gallerai.Infrastructure.Services;
using Gallerai.SharedKernel.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gallerai.Infrastructure.ServiceRegistration;

internal static class CloudflareR2
{
    public static IServiceCollection AddCloudflareR2(this IServiceCollection services, IConfiguration configuration)
    {
        var cloudflareR2Settings = configuration.GetConfiguration<CloudflareR2Settings>();

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

        return services;
    }
}
