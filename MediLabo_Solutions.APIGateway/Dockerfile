# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les fichiers de projets pour la restauration
COPY ["MediLabo_Solutions.APIGateway/MediLabo_Solutions.APIGateway.csproj", "MediLabo_Solutions.APIGateway/"]

RUN dotnet restore "./MediLabo_Solutions.APIGateway/MediLabo_Solutions.APIGateway.csproj"

# Copier tout le code source
COPY . .
WORKDIR "/src/MediLabo_Solutions.APIGateway"
RUN dotnet build "./MediLabo_Solutions.APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MediLabo_Solutions.APIGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Remplacer ocelot.json par la version Docker
RUN rm -f ocelot.json
COPY MediLabo_Solutions.APIGateway/ocelot.Docker.json ./ocelot.json
ENTRYPOINT ["dotnet", "MediLabo_Solutions.APIGateway.dll"]