# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY GameOnTonight/GameOnTonight.csproj GameOnTonight/
COPY GameOnTonight.Infrastructure/GameOnTonight.Infrastructure.csproj GameOnTonight.Infrastructure/
RUN dotnet restore "GameOnTonight/GameOnTonight.csproj"
COPY . .
WORKDIR /src/GameOnTonight
RUN dotnet build "GameOnTonight.csproj" -c Release -o /app/build
RUN dotnet publish "GameOnTonight.csproj" -c Release -o /app/publish

# Étape 2 : Image finale nginx pour servir les fichiers statiques
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY --from=build /app/publish/wwwroot .
# Le fichier nginx.conf est maintenant monté via un volume dans docker-compose.yml

# Expose le port 80
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
