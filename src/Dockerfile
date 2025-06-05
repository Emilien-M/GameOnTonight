# Étape 1 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY GameOnTonight/GameOnTonight.csproj GameOnTonight/
RUN dotnet restore GameOnTonight/GameOnTonight.csproj
COPY GameOnTonight/. GameOnTonight/
WORKDIR /src/GameOnTonight
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : Image finale nginx pour servir les fichiers statiques
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
RUN rm -rf ./*
COPY --from=build /app/publish/wwwroot .
COPY GameOnTonight/nginx.conf /etc/nginx/nginx.conf

# Expose le port 80
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

