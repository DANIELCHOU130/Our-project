# 使用 .NET SDK 建置映像檔
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 複製 csproj 並還原套件
COPY ESGAPI1/ESGAPI1.csproj ESGAPI1/
RUN dotnet restore ESGAPI1/ESGAPI1.csproj

# 複製所有原始碼並建置
COPY ESGAPI1/. ./ESGAPI1/
WORKDIR /app/ESGAPI1
RUN dotnet publish -c Release -o /app/publish

# 使用 .NET Runtime 執行映像檔
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ESGAPI1.dll"]
