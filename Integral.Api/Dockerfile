FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "/"]
COPY ["Integral.Api/Integral.Api.csproj", "Integral.Api/"]
COPY ["SharedKernel/SharedKernel.csproj", "SharedKernel/"]
RUN dotnet restore "Integral.Api/Integral.Api.csproj"
COPY . .
WORKDIR "/src/Integral.Api"
RUN dotnet build "./Integral.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Integral.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Integral.Api.dll"]
