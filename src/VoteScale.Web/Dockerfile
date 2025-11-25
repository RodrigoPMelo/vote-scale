FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os projetos
COPY src/VoteScale.Domain/*.csproj ./src/VoteScale.Domain/
COPY src/VoteScale.Infrastructure/*.csproj ./src/VoteScale.Infrastructure/
COPY src/VoteScale.Web/*.csproj ./src/VoteScale.Web/
COPY src/VoteScale.Worker/*.csproj ./src/VoteScale.Worker/

# Restaura o Web
RUN dotnet restore src/VoteScale.Web/VoteScale.Web.csproj

# Copia o código e publica
COPY src/ ./src/
WORKDIR /app/src/VoteScale.Web
RUN dotnet publish -c Release -o /app/publish

# Roda
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VoteScale.Web.dll"]