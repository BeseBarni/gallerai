FROM vllm/vllm-openai:v0.15.0

RUN pip install --no-cache-dir \
    opentelemetry-sdk \
    opentelemetry-api \
    opentelemetry-exporter-otlp

ENV OTEL_SERVICE_NAME="vllm-inference-server"