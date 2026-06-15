# Consultez https://aka.ms/customizecontainer pour savoir comment personnaliser votre conteneur de débogage et comment Visual Studio utilise ce Dockerfile pour générer vos images afin d’accélérer le débogage.

# Cet index est utilisé lors de l’exécution à partir de VS en mode rapide (par défaut pour la configuration de débogage)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Cette phase est utilisée pour générer le projet de service
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les fichiers de projets pour la restauration
COPY ["MediLabo_Solutions.PatientService/MediLabo_Solutions.PatientService.csproj", "MediLabo_Solutions.PatientService/"]
COPY ["MediLabo_Solutions.ExceptionHandler/MediLabo_Solutions.ExceptionHandler.csproj", "MediLabo_Solutions.ExceptionHandler/"]
COPY ["MediLabo_Solutions.Shared/MediLabo_Solutions.Shared.shproj", "MediLabo_Solutions.Shared/"]
COPY ["MediLabo_Solutions.Shared/MediLabo_Solutions.Shared.projitems", "MediLabo_Solutions.Shared/"]

RUN dotnet restore "./MediLabo_Solutions.PatientService/MediLabo_Solutions.PatientService.csproj"

# Copier tout le code source
COPY . .
WORKDIR "/src/MediLabo_Solutions.PatientService"
RUN dotnet build "./MediLabo_Solutions.PatientService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Cette étape permet de publier le projet de service à copier dans la phase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MediLabo_Solutions.PatientService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Cette phase est utilisée en production ou lors de l’exécution à partir de VS en mode normal (par défaut quand la configuration de débogage n’est pas utilisée)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MediLabo_Solutions.PatientService.dll"]