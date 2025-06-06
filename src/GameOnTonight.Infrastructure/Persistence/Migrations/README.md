# Migrations de Base de Données

Ce dossier contient les migrations Entity Framework Core pour PostgreSQL.

## Créer une nouvelle migration

```bash
dotnet ef migrations add NomDeLaMigration -p ../GameOnTonight.Infrastructure -s ../GameOnTonight.Api
```

## Appliquer les migrations

```bash
dotnet ef database update -p ../GameOnTonight.Infrastructure -s ../GameOnTonight.Api
```
