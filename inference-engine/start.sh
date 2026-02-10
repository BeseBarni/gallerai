#!/bin/bash

set -e # Exit immediately if a command exits with a non-zero status

: "${HF_TOKEN:?❌ Error: HF_TOKEN is not set. Add it to RunPod Env Vars!}"
: "${MODEL_REPO:?❌ Error: MODEL_REPO is not set. Add it to RunPod Env Vars!}"
: "${SERVED_NAME:=gallerai}" # Default to 'gallerai' if not set

echo "🚀 Starting Inference Engine for: $MODEL_REPO"

echo "🔑 Logging into Hugging Face..."
huggingface-cli login --token "$HF_TOKEN"

echo "🔥 Firing up vLLM for $SERVED_NAME..."

exec python3 -m vllm.entrypoints.openai.api_server \
    --model "$MODEL_REPO" \
    --served-model-name "$SERVED_NAME" \
    --trust-remote-code \
    --port 8000 \
    --gpu-memory-utilization 0.95 \
    --max-model-len 8192 \
    --dtype float16