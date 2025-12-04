# 🚀 GameOnTonight - Deployment Guide

This guide will help you self-host GameOnTonight on your own server using Docker.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Quick Start (5 minutes)](#quick-start-5-minutes)
3. [Configuration](#configuration)
4. [Advanced Deployment](#advanced-deployment)
5. [Reverse Proxy Setup](#reverse-proxy-setup)
6. [HTTPS with Let's Encrypt](#https-with-lets-encrypt)
7. [Updating](#updating)
8. [Backup & Restore](#backup--restore)
9. [Troubleshooting](#troubleshooting)

---

## Prerequisites

- **Docker** 20.10+ and **Docker Compose** 2.0+
- A server with at least 1GB RAM
- Ports 80 and 8080 available (or custom ports)

### Verify Docker Installation

```bash
docker --version
docker compose version
```

---

## Quick Start (5 minutes)

### Option A: Using Pre-built Images (Recommended)

No need to clone the repository - just download the compose file:

```bash
# Download docker-compose and .env template
curl -O https://raw.githubusercontent.com/Emilien-M/GameOnTonight/master/docker-compose.ghcr.yml
curl -O https://raw.githubusercontent.com/Emilien-M/GameOnTonight/master/.env.example

# Configure environment
cp .env.example .env
# Edit .env with your settings (see Configuration section)

# Start the application
docker compose -f docker-compose.ghcr.yml up -d
```

### Option B: Build from Source

```bash
# Clone the repository
git clone https://github.com/Emilien-M/GameOnTonight.git
cd GameOnTonight

# Configure environment
cp .env.example .env
# Edit .env with your settings (see Configuration section)

# Build and start
docker compose up -d --build
```

### Configure Required Variables

Edit `.env` and set these **mandatory** values:

```bash
# Generate a secure password
POSTGRES_PASSWORD=your_secure_database_password

# Generate with: openssl rand -base64 32
JWT_SECRET=your_32_character_minimum_secret_key
```

### 5. Access the Application

- **Application**: http://localhost (or http://your-server-ip)
- **API**: http://localhost/api

---

## Configuration

### Environment Variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `POSTGRES_USER` | No | `gameadmin` | PostgreSQL username |
| `POSTGRES_PASSWORD` | **Yes** | - | PostgreSQL password |
| `POSTGRES_DB` | No | `gameontonight` | Database name |
| `JWT_SECRET` | **Yes** | - | JWT signing key (min 32 chars) |
| `JWT_EXPIRY_MINUTES` | No | `60` | Token expiration time |
| `JWT_ISSUER` | No | `GameOnTonight` | JWT issuer |
| `JWT_AUDIENCE` | No | `GameOnTonightClients` | JWT audience |
| `API_BASE_URL` | No | `/api` | API URL (relative path or absolute URL) |
| `APP_PORT` | No | `80` | Application exposed port |
| `AUTO_MIGRATE` | No | `true` | Auto-apply DB migrations |

### Generate Secure Values

```bash
# Generate JWT Secret
openssl rand -base64 32

# Generate Database Password
openssl rand -base64 24
```

---

## Advanced Deployment

### Custom Port

To use a different port, update your `.env`:

```bash
APP_PORT=8000
```

The application will be available at `http://your-server:8000` and the API at `http://your-server:8000/api`.

### View Logs

```bash
# All services
docker compose logs -f

# Specific service
docker compose logs -f api
docker compose logs -f app
docker compose logs -f postgres
```

### Check Service Health

```bash
docker compose ps

# Check App health (includes nginx)
curl http://localhost/health

# Check API health (via reverse proxy)
curl http://localhost/api/health

# Check API readiness (includes database)
curl http://localhost/api/ready
```

---

## Reverse Proxy Setup

The application already includes an nginx reverse proxy that routes `/api` requests to the API container. If you're running behind an external reverse proxy (e.g., for HTTPS), use this configuration:

### Nginx Configuration

```nginx
# /etc/nginx/sites-available/gameontonight
server {
    listen 80;
    server_name games.yourdomain.com;

    # Proxy all requests to the Docker container
    location / {
        proxy_pass http://localhost:80;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

### Traefik Configuration

Add labels to your `docker-compose.override.yml`:

```yaml
services:
  app:
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.gameontonight.rule=Host(`games.yourdomain.com`)"
      - "traefik.http.services.gameontonight.loadbalancer.server.port=80"
```

---

## HTTPS with Let's Encrypt

### Using Certbot (Nginx)

```bash
# Install Certbot
sudo apt install certbot python3-certbot-nginx

# Get certificate for your domain
sudo certbot --nginx -d games.yourdomain.com

# Auto-renewal is configured automatically
```

Then restart:

```bash
docker compose down
docker compose up -d
```

---

## Updating

### Pull Latest Version

```bash
# Stop services
docker compose down

# Pull latest code
git pull origin main

# Rebuild and restart
docker compose build --no-cache
docker compose up -d
```

### Database Migrations

Migrations are applied automatically when `AUTO_MIGRATE=true` (default). The API checks for pending migrations at startup and applies them automatically.

To disable auto-migration, set in your `.env`:

```bash
AUTO_MIGRATE=false
```

---

## Backup & Restore

### Backup Database

```bash
# Create backup
docker compose exec postgres pg_dump -U gameadmin gameontonight > backup_$(date +%Y%m%d_%H%M%S).sql

# Or with compression
docker compose exec postgres pg_dump -U gameadmin gameontonight | gzip > backup_$(date +%Y%m%d_%H%M%S).sql.gz
```

### Restore Database

```bash
# From SQL file
cat backup.sql | docker compose exec -T postgres psql -U gameadmin gameontonight

# From compressed file
gunzip -c backup.sql.gz | docker compose exec -T postgres psql -U gameadmin gameontonight
```

### Full Backup (including volumes)

```bash
# Stop services first
docker compose down

# Backup volume
docker run --rm -v gameontonight-postgres-data:/data -v $(pwd):/backup alpine tar cvf /backup/postgres_data_backup.tar /data

# Restart
docker compose up -d
```

---

## Troubleshooting

### Services Won't Start

```bash
# Check status
docker compose ps

# View logs
docker compose logs

# Check specific service
docker compose logs api --tail 50
```

### Database Connection Issues

```bash
# Test database connection
docker compose exec postgres psql -U gameadmin -d gameontonight -c "SELECT 1"

# Check database logs
docker compose logs postgres
```

### API Returns 503

The API might still be starting. Check:

```bash
# Wait for health check (via reverse proxy)
curl http://localhost/api/ready

# Or directly inside the container
docker compose exec api curl http://localhost:8080/api/ready
```

### Reset Everything

```bash
# Stop and remove everything (including data!)
docker compose down -v

# Start fresh
docker compose up -d
```

### Container Resource Issues

Check if containers have enough resources:

```bash
docker stats
```

---

## Architecture

```
┌───────────────────────────────────────────────────────┐
│                    Docker Network                     │
│                                                       │
│  ┌─────────────────────────────────────────────────┐  │
│  │               App Container (Nginx)             │  │
│  │  ┌──────────────────┐  ┌───────────────────────┐│  │
│  │  │  /               │  │  /api → proxy_pass    ││  │
│  │  │  Blazor WASM     │  │  to API container     ││  │
│  │  │  Static Files    │  │  (internal:8080)      ││  │
│  │  └──────────────────┘  └───────────────────────┘│  │
│  └─────────────────────────────────────────────────┘  │
│        │                         │                    │
│        │ Exposed :80             │ Internal           │
│        │                         ▼                    │
│        │                ┌─────────────────┐           │
│        │                │   API (.NET)    │           │
│        │                │   :8080         │           │
│        │                └────────┬────────┘           │
│        │                         │                    │
│        │                         ▼                    │
│        │                ┌─────────────────┐           │
│        │                │  PostgreSQL     │           │
│        │                │   :5432         │           │
│        │                └─────────────────┘           │
│        │                   Internal only              │
└────────┼──────────────────────────────────────────────┘
         │
    Single exposed port
    (80 or custom APP_PORT)

URL Structure:
  - https://games.yourdomain.com/       → Blazor WASM App
  - https://games.yourdomain.com/api/   → .NET API (via reverse proxy)
```

---

## Support

- 📖 [Documentation](README.md)
- 🐛 [Report Issues](https://github.com/Emilien-M/GameOnTonight/issues)
- 💬 [Discussions](https://github.com/Emilien-M/GameOnTonight/discussions)

---

## License

GameOnTonight is open-source software licensed under the [MIT License](LICENSE).
