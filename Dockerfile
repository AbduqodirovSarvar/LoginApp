# This is your Dockerfile with volume added
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# Define environment variables
ENV ASPNETCORE_URLS=http://+:5100
ENV ConnectionStrings__SQLiteConnection=Data
ENV ASPNETCORE_ENVIRONMENT=Production
ENV Source=/app/DB/Portfolio.db

# Define a volume for the database
VOLUME /app/DB

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LoginApp.csproj", "."]
RUN dotnet restore "./././LoginApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./LoginApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LoginApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoginApp.dll"]
