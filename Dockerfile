# 使用 Microsoft 的 .NET SDK 作為建置階段
FROM mcr.microsoft.comdotnetsdk7.0 AS build
WORKDIR app

# 複製 csproj 並還原
COPY ESGAPI1.csproj .ESGAPI1
RUN dotnet restore ESGAPI1ESGAPI1.csproj

# 複製所有原始碼並建置
COPY . .
RUN dotnet publish ESGAPI1ESGAPI1.csproj -c Release -o out

# 使用 ASP.NET 執行階段作為執行階段映像
FROM mcr.microsoft.comdotnetaspnet7.0
WORKDIR app
COPY --from=build appout .

# 啟動應用程式
ENTRYPOINT [dotnet, ESGAPI1.dll]
