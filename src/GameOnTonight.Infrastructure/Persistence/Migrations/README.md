# Migrations de Base de Données

Ce dossier contient les migrations Entity Framework Core pour PostgreSQL.

## Créer une nouvelle migration

Context : GameOnTonight.Infrastructure.Persistence

```bash
dotnet ef migrations add Init -p ../GameOnTonight.Infrastructure -s ../GameOnTonight.Api -o Persistence/Migrations/
```

## Appliquer les migrations

```bash
dotnet ef database update -p ../GameOnTonight.Infrastructure -s ../GameOnTonight.Api
```
