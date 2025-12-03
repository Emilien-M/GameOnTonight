# ============================================
# GameOnTonight API - Multi-stage Dockerfile
# ============================================

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all project files for proper restore
COPY GameOnTonight.Api/GameOnTonight.Api.csproj ./GameOnTonight.Api/
COPY GameOnTonight.Application/GameOnTonight.Application.csproj ./GameOnTonight.Application/
COPY GameOnTonight.Domain/GameOnTonight.Domain.csproj ./GameOnTonight.Domain/
COPY GameOnTonight.Infrastructure/GameOnTonight.Infrastructure.csproj ./GameOnTonight.Infrastructure/
COPY GameOnTonight.RestClient/GameOnTonight.RestClient.csproj ./GameOnTonight.RestClient/

# Restore dependencies
RUN dotnet restore GameOnTonight.Api/GameOnTonight.Api.csproj

# Copy source code
COPY GameOnTonight.Api/ ./GameOnTonight.Api/
COPY GameOnTonight.Application/ ./GameOnTonight.Application/
COPY GameOnTonight.Domain/ ./GameOnTonight.Domain/
COPY GameOnTonight.Infrastructure/ ./GameOnTonight.Infrastructure/
COPY GameOnTonight.RestClient/ ./GameOnTonight.RestClient/

# Build and publish
WORKDIR /src/GameOnTonight.Api
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

# Install curl for healthcheck
RUN apk add --no-cache curl

# Create non-root user for security
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

# Copy published app
COPY --from=build /app/publish .

# Expose port (cloud convention)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Healthcheck
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "GameOnTonight.Api.dll"]
