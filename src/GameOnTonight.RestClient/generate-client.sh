#!/bin/bash

echo "🚀 Génération du client RestClient depuis l'OpenAPI..."

# URL de votre API (ajustez selon votre configuration)
API_URL="http://localhost:5235/openapi/v1.json"
OUTPUT_PATH="./Generated"

# Vérifiez que l'API est accessible
echo "📡 Vérification de l'accessibilité de l'API..."

if curl -k -f -s -o /dev/null -w "%{http_code}" "$API_URL" | grep -q "200"; then
    echo "✅ API accessible"
else
    echo "❌ Erreur : L'API n'est pas accessible. Assurez-vous qu'elle est démarrée."
    echo "   URL testée : $API_URL"
    exit 1
fi

# Suppression du dossier Generated s'il existe
if [ -d "$OUTPUT_PATH" ]; then
    echo "🗑️  Suppression de l'ancien client..."
    rm -rf "$OUTPUT_PATH"
fi

# Génération du client avec Kiota
echo "⚙️  Génération du client avec Kiota..."

kiota generate \
    --language CSharp \
    --openapi "$API_URL" \
    --class-name GameOnTonightClient \
    --namespace-name GameOnTonight.RestClient \
    --output "$OUTPUT_PATH" \
    --clean-output

if [ $? -eq 0 ]; then
    echo "✅ Client généré avec succès dans $OUTPUT_PATH"
else
    echo "❌ Erreur lors de la génération du client"
    exit 1
fi