# ============================
# Stage 1: Build
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY OrdersWeb.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o /app --no-restore

# ============================
# Stage 2: Runtime
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app ./

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "OrdersWeb.dll"]