FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copia os projetos
COPY src/VoteScale.Domain/*.csproj ./src/VoteScale.Domain/
COPY src/VoteScale.Infrastructure/*.csproj ./src/VoteScale.Infrastructure/
COPY src/VoteScale.Web/*.csproj ./src/VoteScale.Web/
COPY src/VoteScale.Worker/*.csproj ./src/VoteScale.Worker/

# Restaura o Worker
RUN dotnet restore src/VoteScale.Worker/VoteScale.Worker.csproj

# Copia o código e publica
COPY src/ ./src/
WORKDIR /app/src/VoteScale.Worker
RUN dotnet publish -c Release -o /app/publish

# Roda
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VoteScale.Worker.dll"]