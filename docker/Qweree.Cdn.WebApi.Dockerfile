FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
COPY *.sln .
COPY src ./src/
RUN dotnet restore ./src/Qweree.Cdn.WebApi/Qweree.Cdn.WebApi.csproj
RUN dotnet publish ./src/Qweree.Cdn.WebApi/Qweree.Cdn.WebApi.csproj -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "/app/Qweree.Cdn.WebApi.dll"]