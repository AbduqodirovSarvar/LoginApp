FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Define environment variables
ENV ConnectionStrings__SQLiteConnection="Data Source=/app/DB/LoginApp.db" 
ENV ASPNETCORE_ENVIRONMENT=Production

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LoginApp.csproj", "."]
RUN dotnet restore "./LoginApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./LoginApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./LoginApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN chmod -R u+w /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Change ownership of the /app/DB directory to the app user
RUN chown -R app:app /app/DB
USER app
ENTRYPOINT ["dotnet", "LoginApp.dll"]
