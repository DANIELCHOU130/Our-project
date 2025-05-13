FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ESGAPI1/ESGAPI1.csproj ./ESGAPI1/
RUN dotnet restore ESGAPI1/ESGAPI1.csproj

COPY ESGAPI1/. ./ESGAPI1/
RUN dotnet publish ESGAPI1/ESGAPI1.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "ESGAPI1.dll"]
