FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinacialProjectVersion3/FinacialProjectVersion3.csproj", "FinacialProjectVersion3/"]
RUN dotnet restore "FinacialProjectVersion3/FinacialProjectVersion3.csproj"
COPY . .
WORKDIR "/src/FinacialProjectVersion3"
RUN dotnet build "./FinacialProjectVersion3.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinacialProjectVersion3.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinacialProjectVersion3.dll"]
