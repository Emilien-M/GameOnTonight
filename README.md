# Game on Tonight 

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![Project Status](https://img.shields.io/badge/status-in%20development-green.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED)

An open source app project to help you choose the next board game to take out of your collection. No more spending 30 minutes debating—let the app choose for you!

## Description

This application allows a user to catalog their personal board game collection. During a game night, simply enter a few criteria (number of players, available time, type of game, etc.) and the app will filter your collection and suggest a list of compatible games.

This project is actively developed. Feel free to suggest ideas or report bugs!

---

## 🚀 Quick Start (Self-Hosting)

Deploy GameOnTonight on your own server in 5 minutes:

```bash
# 1. Clone the repository
git clone https://github.com/Emilien-M/GameOnTonight.git
cd GameOnTonight

# 2. Configure environment
cp .env.example .env
# Edit .env and set POSTGRES_PASSWORD and JWT_SECRET

# 3. Start the application
docker compose up -d

# 4. Access the app
# Frontend: http://localhost
# API: http://localhost:8080
```

### Generate Secure Secrets

```bash
# JWT Secret (required)
openssl rand -base64 32

# Database Password (required)
openssl rand -base64 24
```

📖 **[Full Deployment Guide](DEPLOYMENT.md)** - Includes reverse proxy, HTTPS, backups, and more.

---

## [Roadmap](ROADMAP.md)

## How to Contribute

Contributions are welcome! If you want to help, here’s how you can do it:

1.  **Report bugs or suggest features:** Open an "Issue" on GitHub and describe the problem or your idea as precisely as possible.
2.  **Propose code improvements:**
    * Fork the project.
    * Create a new branch (`git checkout -b feature/FeatureName`).
    * Make your changes.
    * Submit a "Pull Request" so we can integrate it.

## Tech Stack

* **Frontend:** Blazor WebAssembly (PWA)
* **Backend:** .NET 9 Web API (Clean Architecture)
* **Database:** PostgreSQL 16
* **Authentication:** ASP.NET Core Identity with JWT
* **UI Framework:** MudBlazor
* **Containerization:** Docker & Docker Compose

## License

This project is distributed under the MIT license. See the `LICENSE` file for more details.