import { imagesUploadedEndpoint } from '@shared/src/api/worker/worker.gen';
import { setBaseUrl } from '@shared/src/lib/worker-client';

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

		const eventsToSend = [];

		for (const message of batch.messages) {
			const r2Event = message.body;

			// Filter only for uploads
			if (r2Event.action === 'PutObject') {
				eventsToSend.push({
					key: r2Event.object.key,
					size: r2Event.object.size,
					bucket: r2Event.bucket,
					timestamp: new Date().toISOString(),
				});
			}
		}
		
		if (eventsToSend.length === 0) {
			batch.ackAll();
			return;
		}

		try {
			const response = await imagesUploadedEndpoint({ events: eventsToSend });

			if (response.data.isSuccess) {
				batch.ackAll();
			} else {

				batch.retryAll();
			}
		} catch (error) {

			batch.retryAll();
		}
	},

	async fetch(request: Request, env: Env, ctx: ExecutionContext): Promise<Response> {
		return new Response("I am a background Queue Worker. I don't speak HTTP! 👻", {
			status: 404,
		});
	},
};
