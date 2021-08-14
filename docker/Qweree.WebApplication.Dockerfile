FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY *.sln .
COPY src ./src/

ARG APPSETTINGS_PATH=src/Qweree.WebApplication/wwwroot/appsettings.json
COPY $APPSETTINGS_PATH ./src/Qweree.WebApplication/wwwroot/appsettings.json

RUN dotnet restore ./src/Qweree.WebApplication/Qweree.WebApplication.csproj
RUN dotnet build ./src/Qweree.WebApplication/Qweree.WebApplication.csproj -c Release -o publish --no-restore

FROM build-env AS publish
RUN dotnet publish ./src/Qweree.WebApplication/Qweree.WebApplication.csproj -c Release -o /publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish /publish/wwwroot /usr/local/webapp/nginx/html
COPY ./src/Qweree.WebApplication/nginx/nginx.conf /etc/nginx/nginx.conf