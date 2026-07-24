FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ChroniclesAPI/ChroniclesAPI.csproj", "ChroniclesAPI/"]
COPY ["PlayerApp.Models/PlayerApp.Models.csproj", "PlayerApp.Models/"]
RUN dotnet restore "ChroniclesAPI/ChroniclesAPI.csproj"
COPY . .
WORKDIR "/src/ChroniclesAPI"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ChroniclesAPI.dll"]