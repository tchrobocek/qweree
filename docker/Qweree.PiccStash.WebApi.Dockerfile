FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
COPY *.sln .
COPY src ./src/
RUN dotnet restore ./src/Qweree.PiccStash.WebApi/Qweree.PiccStash.WebApi.csproj
RUN dotnet publish ./src/Qweree.PiccStash.WebApi/Qweree.PiccStash.WebApi.csproj -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "/app/Qweree.PiccStash.WebApi.dll"]