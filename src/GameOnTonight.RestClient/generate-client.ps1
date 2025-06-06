#!/usr/bin/env pwsh
#Requires -Version 7.0

param(
    [string]$ApiUrl = "http://localhost:5235",
    [string]$OpenApiEndpoint = "/openapi/v1.json",
    [string]$OutputFile = "./Generated/ApiClient.cs",
    [string]$Namespace = "GameOnTonight.RestClient.Generated",
    [string]$ClientName = "GameOnTonightApiClient"
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

Write-Separator "GÉNÉRATION DU CLIENT API GAMESONTONIGHT"

Write-Host "Configuration:"
Write-Host "  API URL:         $ApiUrl" -ForegroundColor Cyan
Write-Host "  OpenAPI Endpoint: $OpenApiEndpoint" -ForegroundColor Cyan
Write-Host "  Fichier de sortie: $OutputFile" -ForegroundColor Cyan
Write-Host "  Namespace:        $Namespace" -ForegroundColor Cyan
Write-Host "  Nom du client:    $ClientName" -ForegroundColor Cyan
Write-Host ""

# Créer le dossier Generated s'il n'existe pas
$generatedDir = Split-Path -Path $OutputFile -Parent
if (-not (Test-Path -Path $generatedDir)) {
    New-Item -ItemType Directory -Path $generatedDir | Out-Null
    Write-Host "✓ Dossier de sortie créé: $generatedDir" -ForegroundColor Green
}
else {
    Write-Host "✓ Dossier de sortie existant: $generatedDir" -ForegroundColor Green
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
            
            # Utilisation de GET au lieu de HEAD
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

Write-Separator "VÉRIFICATION DE NSWAG"

# Vérification et installation de NSwag si nécessaire
try {
    $nswagGlobalInstalled = $false
    $nswagLocalInstalled = $false
    $nswagVersion = ""
    
    # Vérification de l'installation globale de NSwag
    try {
        Write-Host "Recherche de NSwag en installation globale..." -NoNewline
        $globalNswag = Invoke-Expression "dotnet tool list -g" | Select-String "nswag.consolecor"
        if ($globalNswag) {
            $nswagGlobalInstalled = $true
            $nswagVersion = $globalNswag.ToString().Trim()
            Write-Host " TROUVÉ" -ForegroundColor Green
            Write-Host "  $nswagVersion" -ForegroundColor Cyan
        } else {
            Write-Host " NON TROUVÉ" -ForegroundColor Yellow
        }
    } catch {
        Write-Host " ERREUR" -ForegroundColor Red
        Write-Host "  Impossible de vérifier l'installation globale: $($_.Exception.Message)" -ForegroundColor Yellow
    }
    
    # Vérification de l'installation locale de NSwag
    if (-not $nswagGlobalInstalled) {
        try {
            Write-Host "Recherche de NSwag en installation locale..." -NoNewline
            $localNswag = Invoke-Expression "dotnet tool list" | Select-String "nswag.consolecor"
            if ($localNswag) {
                $nswagLocalInstalled = $true
                $nswagVersion = $localNswag.ToString().Trim()
                Write-Host " TROUVÉ" -ForegroundColor Green
                Write-Host "  $nswagVersion" -ForegroundColor Cyan
            } else {
                Write-Host " NON TROUVÉ" -ForegroundColor Yellow
            }
        } catch {
            Write-Host " ERREUR" -ForegroundColor Red
            Write-Host "  Impossible de vérifier l'installation locale: $($_.Exception.Message)" -ForegroundColor Yellow
        }
    }
    
    # Installation de NSwag si nécessaire
    if (-not $nswagGlobalInstalled -and -not $nswagLocalInstalled) {
        Write-Host "NSwag n'est pas installé. Installation nécessaire." -ForegroundColor Yellow
        
        # Vérifier si un fichier manifeste existe déjà
        if (-not (Test-Path ".config/dotnet-tools.json")) {
            Write-Host "Création du fichier manifeste pour les outils locaux..." -NoNewline
            Invoke-Expression "dotnet new tool-manifest" | Out-Null
            if ($LASTEXITCODE -eq 0) {
                Write-Host " OK" -ForegroundColor Green
            } else {
                Write-Host " ÉCHEC" -ForegroundColor Red
                throw "Échec de la création du fichier manifeste d'outils."
            }
        }
        
        # Installation de NSwag en local
        Write-Host "Installation de NSwag.ConsoleCore en local..." -NoNewline
        Invoke-Expression "dotnet tool install NSwag.ConsoleCore" | Out-Null
        
        if ($LASTEXITCODE -eq 0) {
            $nswagLocalInstalled = $true
            Write-Host " SUCCÈS" -ForegroundColor Green
            $localNswag = Invoke-Expression "dotnet tool list" | Select-String "nswag.consolecor"
            if ($localNswag) {
                $nswagVersion = $localNswag.ToString().Trim()
                Write-Host "  $nswagVersion" -ForegroundColor Cyan
            }
        } else {
            Write-Host " ÉCHEC" -ForegroundColor Red
            Write-Host "Tentative d'installation globale..." -NoNewline
            
            Invoke-Expression "dotnet tool install -g NSwag.ConsoleCore" | Out-Null
            
            if ($LASTEXITCODE -eq 0) {
                $nswagGlobalInstalled = $true
                Write-Host " SUCCÈS" -ForegroundColor Green
                $globalNswag = Invoke-Expression "dotnet tool list -g" | Select-String "nswag.consolecor"
                if ($globalNswag) {
                    $nswagVersion = $globalNswag.ToString().Trim()
                    Write-Host "  $nswagVersion" -ForegroundColor Cyan
                }
            } else {
                Write-Host " ÉCHEC" -ForegroundColor Red
                throw "Échec de l'installation de NSwag."
            }
        }
    }
    
    Write-Separator "GÉNÉRATION DU CLIENT"
    
    # Construction de la commande NSwag
    if ($nswagLocalInstalled) {
        $commandBase = "dotnet nswag"
    } else {
        $commandBase = "nswag"
    }

    # Construire la commande complète
    $command = "$commandBase openapi2csclient /input:`"$openApiUrl`" /output:`"$OutputFile`" /namespace:$Namespace /className:$ClientName /GenerateClientInterfaces:true /UseBaseUrl:false /GenerateExceptionClasses:true /ExceptionClass:ApiException"

    Write-Host ""
    Write-Host "Commande de génération:" -ForegroundColor Cyan
    Write-Host $command -ForegroundColor DarkCyan
    Write-Host ""
    Write-Host "Exécution de la commande..." -ForegroundColor Yellow
    
    # Exécution de la commande NSwag
    $startTime = Get-Date
    Invoke-Expression $command
    $endTime = Get-Date
    $duration = $endTime - $startTime
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✓ Client API généré avec succès!" -ForegroundColor Green
        Write-Host "  Fichier: $OutputFile" -ForegroundColor Cyan
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
