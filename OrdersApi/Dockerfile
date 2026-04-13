# ============================
# Stage 1: Build
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia só o .csproj primeiro pra aproveitar cache de restore
COPY OrdersApi.csproj ./
RUN dotnet restore

# Agora copia o resto do código-fonte
COPY . ./

# Publica em modo Release pra pasta /app
RUN dotnet publish -c Release -o /app --no-restore

# ============================
# Stage 2: Runtime
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copia só os binários publicados da stage anterior
COPY --from=build /app ./

# Porta que o app escuta dentro do container
EXPOSE 8080

# Em runtime, a porta do Kestrel vem por env var — definimos 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OrdersApi.dll"]