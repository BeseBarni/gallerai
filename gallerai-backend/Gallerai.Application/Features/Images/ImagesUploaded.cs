using Gallerai.Application.Interfaces;
using Gallerai.SharedKernel.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gallerai.Application.Features.Images;

public static class ImagesUploaded
{
    public record ImageUploadedEvent(string Key, long Size, string Bucket, DateTime Timestamp);
    public record Request(ImageUploadedEvent[] Events);
    public record Command(ImageUploadedEvent[] Events) : IRequest<Result<Response>>;
    public record Response(int ProcessedCount, int FailedCount, string[] FailedKeys);

    public sealed class Handler(IGalleraiDbContext context) : IRequestHandler<Command, Result<Response>>
    {
        public async Task<Result<Response>> Handle(Command request, CancellationToken ct)
        {
            var processedCount = 0;
            var failedKeys = new List<string>();

            var keys = request.Events.Select(e => e.Key).ToArray();
            var images = await context.Images
                .Where(i => i.R2Key != null && keys.Contains(i.R2Key))
                .ToDictionaryAsync(i => i.R2Key!, ct);

            foreach (var uploadEvent in request.Events)
            {
                if (images.TryGetValue(uploadEvent.Key, out var image))
                {
                    image.MarkAsUploaded(uploadEvent.Size, uploadEvent.Timestamp);
                    processedCount++;
                }
                else
                {
                    failedKeys.Add(uploadEvent.Key);
                }
            }

            await context.SaveChangesAsync(ct);

            return new Response(processedCount, failedKeys.Count, [.. failedKeys]);
        }
    }
}
