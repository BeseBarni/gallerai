import { imagesUploadedEndpoint } from '@shared/src/api/worker/worker.gen';

interface Env {
	IMAGES_BUCKET: R2Bucket;
	BACKEND_URL: string;
	BACKEND_SECRET: string; // Set via `wrangler secret put`
}

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
		const eventsToSend = [];

		// 1. Unpack the Queue Batch
		for (const message of batch.messages) {
			// R2 events are inside message.body
			const r2Event = message.body;

			// Filter only for uploads (PutObject)
			if (r2Event.action === 'PutObject') {
				eventsToSend.push({
					key: r2Event.object.key,
					size: r2Event.object.size,
					bucket: r2Event.bucket,
					timestamp: new Date().toISOString(),
				});
			}

			// Explicitly acknowledge this message so it doesn't get retried
			message.ack();
		}

		if (eventsToSend.length === 0) return;

		// 2. Send Batch to .NET Backend
		try {
			const response = await imagesUploadedEndpoint(eventsToSend);

			if (!response.ok) {
				throw new Error(`Backend responded with ${response.status}`);
			}

			console.log(`Successfully notified backend of ${eventsToSend.length} uploads.`);
		} catch (error) {
			console.error('Backend notification failed:', error);
			// If the backend is down, we FAIL the batch.
			// Cloudflare will automatically retry these messages later.
			batch.retryAll();
		}
	},
};
