#!/usr/bin/env pwsh
#Requires -Version 7.0

param(
    [string]$ApiUrl = "http://localhost:5235",
    [string]$OpenApiEndpoint = "/openapi/v1.json",
    [string]$OutputFolder = "./Generated",
    [string]$Namespace = "GameOnTonight.AutoRest.Generated",
    [string]$ClientName = "GameOnTonightClient"
)

# Fonction pour afficher des séparateurs visuels dans les logs
function Write-Separator {
    param ([string]$Title = "")
    
    $separatorLine = "=" * 60
    Write-Host ""
    Write-Host $separatorLine -ForegroundColor DarkCyan
    if ($Title) {
        Write-Host "  $Title" -ForegroundColor Cyan
        Write-Host $separatorLine -ForegroundColor DarkCyan
    }
    Write-Host ""
}

Write-Separator "GÉNÉRATION DU CLIENT API GAMESONTONIGHT AVEC AUTOREST (AZURE.CORE)"

Write-Host "Configuration:"
Write-Host "  API URL:          $ApiUrl" -ForegroundColor Cyan
Write-Host "  OpenAPI Endpoint: $OpenApiEndpoint" -ForegroundColor Cyan
Write-Host "  Dossier de sortie: $OutputFolder" -ForegroundColor Cyan
Write-Host "  Namespace:        $Namespace" -ForegroundColor Cyan
Write-Host "  Nom du client:    $ClientName" -ForegroundColor Cyan
Write-Host ""

# Créer le dossier Generated s'il n'existe pas
if (-not (Test-Path -Path $OutputFolder)) {
    New-Item -ItemType Directory -Path $OutputFolder | Out-Null
    Write-Host "✓ Dossier de sortie créé: $OutputFolder" -ForegroundColor Green
}
else {
    Write-Host "✓ Dossier de sortie existant: $OutputFolder" -ForegroundColor Green
}

# URL complète pour le document OpenAPI
$openApiUrl = "$ApiUrl$OpenApiEndpoint"
Write-Host "URL OpenAPI: $openApiUrl" -ForegroundColor Yellow

Write-Separator "VÉRIFICATION DE L'API"

# Vérification de la disponibilité de l'API
try {
    $apiAvailable = $false
    $attempts = 0
    $maxAttempts = 3
    
    while (-not $apiAvailable -and $attempts -lt $maxAttempts) {
        try {
            $attempts++
            Write-Host "Tentative de connexion à l'API ($attempts/$maxAttempts)..." -NoNewline
            
            # Utilisation de GET pour récupérer le document OpenAPI
            $response = Invoke-RestMethod -Uri $openApiUrl -Method Get -TimeoutSec 10
            $apiAvailable = $true
            
            Write-Host " SUCCÈS" -ForegroundColor Green
            Write-Host ""
            
            # Afficher des informations sur le document OpenAPI si disponibles
            if ($response.openapi) {
                Write-Host "Informations OpenAPI:" -ForegroundColor Cyan
                Write-Host "  Version:     $($response.openapi)" -ForegroundColor White
                if ($response.info) {
                    Write-Host "  Titre:       $($response.info.title)" -ForegroundColor White
                    Write-Host "  Description: $($response.info.description)" -ForegroundColor White
                    Write-Host "  Version API: $($response.info.version)" -ForegroundColor White
                }
                # Nombre d'endpoints (approximation)
                $endpointCount = 0
                if ($response.paths) {
                    foreach ($path in $response.paths.PSObject.Properties) {
                        foreach ($method in $path.Value.PSObject.Properties) {
                            $endpointCount++
                        }
                    }
                    Write-Host "  Endpoints:    $endpointCount détectés" -ForegroundColor White
                }
            }
        }
        catch {
            Write-Host " ÉCHEC" -ForegroundColor Red
            Write-Host "  Erreur: $($_.Exception.Message)" -ForegroundColor Red
            
            if ($attempts -lt $maxAttempts) {
                Write-Host "  Nouvelle tentative dans 5 secondes..." -ForegroundColor Yellow
                Start-Sleep -Seconds 5
            }
            else {
                throw "Impossible de se connecter à l'API après $maxAttempts tentatives."
            }
        }
    }
}
catch {
    Write-Host "✘ ERREUR CRITIQUE: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Télécharger le document OpenAPI en local si nécessaire
$swaggerFilePath = Join-Path $PSScriptRoot "swagger.json"
try {
    Write-Host "Téléchargement du document OpenAPI..." -NoNewline
    $response | ConvertTo-Json -Depth 100 | Out-File $swaggerFilePath -Encoding utf8
    Write-Host " SUCCÈS" -ForegroundColor Green
    Write-Host "  Document OpenAPI sauvegardé: $swaggerFilePath" -ForegroundColor White
}
catch {
    Write-Host " ÉCHEC" -ForegroundColor Red
    Write-Host "  Erreur: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Separator "VÉRIFICATION DE AUTOREST"

# Vérifier que AutoRest est installé
try {
    $autorestVersion = ""
    
    Write-Host "Vérification d'AutoRest..." -NoNewline
    $autorestVersionOutput = autorest --version
    
    if ($LASTEXITCODE -eq 0) {
        $autorestVersion = ($autorestVersionOutput -match "@autorest/core\s+([0-9\.]+)")[0]
        if ($autorestVersion) {
            Write-Host " TROUVÉ" -ForegroundColor Green
            Write-Host "  $autorestVersion" -ForegroundColor Cyan
        }
        else {
            Write-Host " VERSION NON IDENTIFIABLE" -ForegroundColor Yellow
        }
    } else {
        Write-Host " NON TROUVÉ" -ForegroundColor Red
        throw "AutoRest n'est pas installé. Installez-le avec 'npm install -g autorest'"
    }
    
    Write-Separator "GÉNÉRATION DU CLIENT"

    # Nettoyer le dossier de sortie
    if (Test-Path -Path $OutputFolder) {
        Write-Host "Nettoyage du dossier de sortie..." -NoNewline
        Remove-Item -Recurse -Force (Join-Path $OutputFolder "*") -ErrorAction SilentlyContinue
        Write-Host " SUCCÈS" -ForegroundColor Green
    }
    
    # Exécuter AutoRest avec le nouveau générateur
    $command = "autorest autorest.yaml"

    Write-Host ""
    Write-Host "Commande de génération:" -ForegroundColor Cyan
    Write-Host $command -ForegroundColor DarkCyan
    Write-Host ""
    Write-Host "Exécution de la commande (cela peut prendre quelques minutes)..." -ForegroundColor Yellow
    
    # Exécution de la commande AutoRest
    $startTime = Get-Date
    Invoke-Expression $command
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✓ Client API généré avec succès!" -ForegroundColor Green
        Write-Host "  Dossier: $OutputFolder" -ForegroundColor Cyan
        Write-Host "  Durée: $($duration.TotalSeconds.ToString("0.00")) secondes" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "✘ Échec de la génération du client API!" -ForegroundColor Red
        Write-Host "  Code de sortie: $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    
    Write-Separator "PROCESSUS TERMINÉ"
}
catch {
    Write-Host ""
    Write-Host "✘ ERREUR CRITIQUE" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host $_.Exception.StackTrace -ForegroundColor DarkRed
    exit 1
}
