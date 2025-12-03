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

- **Frontend**: http://localhost (or http://your-server-ip)
- **API**: http://localhost:8080

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
| `CORS_ORIGIN` | No | `http://localhost` | Frontend origin for CORS |
| `API_BASE_URL` | No | `http://localhost:8080` | Public API URL |
| `API_PORT` | No | `8080` | API exposed port |
| `APP_PORT` | No | `80` | Frontend exposed port |
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

### Custom Ports

To use different ports, update your `.env`:

```bash
API_PORT=3000
APP_PORT=8000
API_BASE_URL=http://your-server:3000
```

### Production with Custom Domain

```bash
CORS_ORIGIN=https://games.yourdomain.com
API_BASE_URL=https://api.games.yourdomain.com
```

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

# Check API health
curl http://localhost:8080/health

# Check API readiness (includes database)
curl http://localhost:8080/ready
```

---

## Reverse Proxy Setup

### Nginx Configuration

If you're running behind Nginx, use this configuration:

```nginx
# /etc/nginx/sites-available/gameontonight
server {
    listen 80;
    server_name games.yourdomain.com;

    # Frontend
    location / {
        proxy_pass http://localhost:80;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}

server {
    listen 80;
    server_name api.games.yourdomain.com;

    # API
    location / {
        proxy_pass http://localhost:8080;
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
  api:
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.api.rule=Host(`api.games.yourdomain.com`)"
      - "traefik.http.services.api.loadbalancer.server.port=8080"

  app:
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.app.rule=Host(`games.yourdomain.com`)"
      - "traefik.http.services.app.loadbalancer.server.port=80"
```

---

## HTTPS with Let's Encrypt

### Using Certbot (Nginx)

```bash
# Install Certbot
sudo apt install certbot python3-certbot-nginx

# Get certificates
sudo certbot --nginx -d games.yourdomain.com -d api.games.yourdomain.com

# Auto-renewal is configured automatically
```

### Update Environment

After enabling HTTPS, update your `.env`:

```bash
CORS_ORIGIN=https://games.yourdomain.com
API_BASE_URL=https://api.games.yourdomain.com
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
# Wait for health check
docker compose exec api curl http://localhost:8080/ready
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
┌─────────────────────────────────────────────────────────┐
│                    Docker Network                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────┐  │
│  │    App      │  │    API      │  │   PostgreSQL    │  │
│  │   (Nginx)   │  │  (.NET 9)   │  │      (16)       │  │
│  │   :80       │──│   :8080     │──│     :5432       │  │
│  └─────────────┘  └─────────────┘  └─────────────────┘  │
│        │                │                   │           │
└────────┼────────────────┼───────────────────┼───────────┘
         │                │                   │
    Exposed          Exposed            Internal only
    Port 80         Port 8080
```

---

## Support

- 📖 [Documentation](README.md)
- 🐛 [Report Issues](https://github.com/Emilien-M/GameOnTonight/issues)
- 💬 [Discussions](https://github.com/Emilien-M/GameOnTonight/discussions)

---

## License

GameOnTonight is open-source software licensed under the [MIT License](LICENSE).
