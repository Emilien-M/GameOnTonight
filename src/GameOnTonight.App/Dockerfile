# ============================================
# GameOnTonight App - Multi-stage Dockerfile
# Blazor WebAssembly served by Nginx
# ============================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files for proper restore
COPY GameOnTonight.App/GameOnTonight.App.csproj ./GameOnTonight.App/
COPY GameOnTonight.RestClient/GameOnTonight.RestClient.csproj ./GameOnTonight.RestClient/
COPY GameOnTonight.Domain/GameOnTonight.Domain.csproj ./GameOnTonight.Domain/
COPY GameOnTonight.Application/GameOnTonight.Application.csproj ./GameOnTonight.Application/

# Restore dependencies
RUN dotnet restore GameOnTonight.App/GameOnTonight.App.csproj

# Copy source code
COPY GameOnTonight.App/ ./GameOnTonight.App/
COPY GameOnTonight.RestClient/ ./GameOnTonight.RestClient/
COPY GameOnTonight.Domain/ ./GameOnTonight.Domain/
COPY GameOnTonight.Application/ ./GameOnTonight.Application/

# Build and publish
WORKDIR /src/GameOnTonight.App
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime with Nginx
FROM nginx:alpine AS runtime

# Install envsubst for environment variable substitution
RUN apk add --no-cache bash

# Copy published Blazor app
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Copy Nginx configuration
COPY GameOnTonight.App/nginx.conf /etc/nginx/nginx.conf

# Copy entrypoint script for runtime configuration
COPY GameOnTonight.App/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Expose port 80
EXPOSE 80

# Healthcheck
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost/health || exit 1

ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"]
