# 使用官方 .NET SDK 作為基礎映像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 設定工作目錄
WORKDIR /app

# 複製 csproj 並還原
COPY ESGAPI1/ESGAPI1/ESGAPI1.csproj ./ESGAPI1/ESGAPI1/

RUN dotnet restore ESGAPI1/ESGAPI1/ESGAPI1.csproj

# 複製其餘的專案檔案
COPY . .

# 建立專案
RUN dotnet publish ESGAPI1/ESGAPI1/ESGAPI1.csproj -c Release -o out

# 使用 ASP.NET 作為基礎映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/out .

# 設置容器啟動命令
ENTRYPOINT ["dotnet", "ESGAPI1.dll"]
