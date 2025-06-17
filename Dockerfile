# 使用官方 .NET SDK 作為建置階段基礎映像
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# 複製 .csproj 並還原 NuGet 套件
COPY ./ESGAPI1/ESGAPI1.csproj ./ESGAPI1/
RUN dotnet restore ./ESGAPI1/ESGAPI1.csproj

# 複製所有來源程式碼
COPY ./ESGAPI1 ./ESGAPI1

# 發行建置
RUN dotnet publish ./ESGAPI1/ESGAPI1.csproj -c Release -o /out

# 建立執行階段映像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

ENTRYPOINT ["dotnet", "ESGAPI1.dll"]
