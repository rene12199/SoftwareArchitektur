FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SoftwareArchitektur.ArchitekturSuggester/SoftwareArchitektur.ArchitekturSuggester.csproj", "SoftwareArchitektur.ArchitekturSuggester/"]
RUN dotnet restore "SoftwareArchitektur.ArchitekturSuggester/SoftwareArchitektur.ArchitekturSuggester.csproj"
COPY . .
WORKDIR "/src/SoftwareArchitektur.ArchitekturSuggester"
RUN dotnet build "SoftwareArchitektur.ArchitekturSuggester.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SoftwareArchitektur.ArchitekturSuggester.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SoftwareArchitektur.ArchitekturSuggester.dll"]
