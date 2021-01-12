FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
COPY *.sln .
COPY src ./src/
RUN dotnet restore ./src/Qweree.Qwill.WebApi/Qweree.Qwill.WebApi.csproj
RUN dotnet publish ./src/Qweree.Qwill.WebApi/Qweree.Qwill.WebApi.csproj -c Release -o publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "/app/Qweree.Qwill.WebApi.dll"]