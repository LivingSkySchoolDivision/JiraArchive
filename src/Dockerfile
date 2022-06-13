FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["JiraArchive/JiraArchive.csproj", "JiraArchive/"]
RUN dotnet restore "JiraArchive/JiraArchive.csproj"
COPY . .
WORKDIR "/src/JiraArchive"
RUN dotnet build "JiraArchive.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "JiraArchive.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "JiraArchive.dll"]