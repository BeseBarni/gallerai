FROM vllm/vllm-openai:latest

WORKDIR /app

COPY start.sh /app/start.sh
RUN chmod +x /app/start.sh

ENV HF_HOME="/root/.cache/huggingface"
ENV PORT=8000

ENTRYPOINT ["/app/start.sh"]