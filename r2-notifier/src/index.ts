import { imagesUploadedEndpoint } from '@shared/src/api/worker/worker.gen';
import { setBaseUrl } from '@shared/src/lib/worker-client';
import type { GalleraiApplicationFeaturesImagesImagesUploadedImageUploadedR2 } from '@shared/src/api/schemas';

interface R2EventNotification {
	object: {
		key: string;
		size: number;
		eTag: string;
	};
	bucket: string;
	action: 'PutObject' | 'DeleteObject';
}

export default {
	async queue(batch: MessageBatch<R2EventNotification>, env: Env): Promise<void> {
		setBaseUrl(env.BACKEND_URL);

		const eventsToSend: GalleraiApplicationFeaturesImagesImagesUploadedImageUploadedR2[] = [];
		const uploadMessages: Message<R2EventNotification>[] = [];

		for (const message of batch.messages) {
			const r2Event = message.body;

			// Filter only for uploads
			if (r2Event.action === 'PutObject') {
				let traceparent = '';

				try {
					const object = await env.IMAGES_BUCKET.head(r2Event.object.key);
					traceparent = object?.customMetadata?.['traceparent'] ?? '';
				} catch (error) {
					// If we can't read metadata for this object, continue with an empty traceparent
					console.error('Failed to read R2 object metadata for traceparent', {
						key: r2Event.object.key,
						error,
					});
				}
				eventsToSend.push({
					key: r2Event.object.key,
					size: r2Event.object.size,
					bucket: r2Event.bucket,
					timestamp: new Date().toISOString(),
					traceparent,
				});
				uploadMessages.push(message);
			} else {
				// Immediately ack non-upload events
				message.ack();
			}
		}

		if (eventsToSend.length === 0) {
			return;
		}

		try {
			const response = await imagesUploadedEndpoint({ events: eventsToSend });

			if (response.data.isSuccess) {
				uploadMessages.forEach((msg) => msg.ack());
			} else {
				uploadMessages.forEach((msg) => msg.retry());
			}
		} catch (error) {
			uploadMessages.forEach((msg) => msg.retry());
		}
	},

	async fetch(request: Request, env: Env, ctx: ExecutionContext): Promise<Response> {
		return new Response("I am a background Queue Worker. I don't speak HTTP! 👻", {
			status: 404,
		});
	},
};
