# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY GameOnTonight.csproj .
RUN dotnet restore GameOnTonight.csproj

# Copy everything else and build
COPY . .
RUN dotnet publish GameOnTonight.csproj -c Release -o /app/publish

# Final stage
FROM nginx:alpine
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
