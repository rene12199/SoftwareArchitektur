FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SoftwareArchitektur.ExcelDataReader/SoftwareArchitektur.ExcelDataReader.csproj", "SoftwareArchitektur.ExcelDataReader/"]
RUN dotnet restore "SoftwareArchitektur.ExcelDataReader/SoftwareArchitektur.ExcelDataReader.csproj"
COPY . .
WORKDIR "/src/SoftwareArchitektur.ExcelDataReader"
RUN dotnet build "SoftwareArchitektur.ExcelDataReader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SoftwareArchitektur.ExcelDataReader.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SoftwareArchitektur.ExcelDataReader.dll"]
