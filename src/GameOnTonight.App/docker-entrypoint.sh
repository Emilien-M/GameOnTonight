#!/bin/bash
set -e

# ============================================
# GameOnTonight App - Docker Entrypoint
# Injects API_BASE_URL at runtime
# ============================================

CONFIG_FILE="/usr/share/nginx/html/appsettings.json"

# Default API URL - use relative path /api (served via nginx reverse proxy)
API_BASE_URL="${API_BASE_URL:-/api}"

# Create or update appsettings.json with the API URL
echo "{\"ApiBaseUrl\": \"${API_BASE_URL}\"}" > "$CONFIG_FILE"

echo "✅ Configuration injected: API_BASE_URL=${API_BASE_URL}"

# Execute the main command (nginx)
exec "$@"
