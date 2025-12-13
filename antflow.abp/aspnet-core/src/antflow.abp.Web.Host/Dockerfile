FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/antflow.abp.Web.Host/antflow.abp.Web.Host.csproj", "src/antflow.abp.Web.Host/"]
COPY ["src/antflow.abp.Web.Core/antflow.abp.Web.Core.csproj", "src/antflow.abp.Web.Core/"]
COPY ["src/antflow.abp.Application/antflow.abp.Application.csproj", "src/antflow.abp.Application/"]
COPY ["src/antflow.abp.Core/antflow.abp.Core.csproj", "src/antflow.abp.Core/"]
COPY ["src/antflow.abp.EntityFrameworkCore/antflow.abp.EntityFrameworkCore.csproj", "src/antflow.abp.EntityFrameworkCore/"]
WORKDIR "/src/src/antflow.abp.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/antflow.abp.Web.Host", "src/antflow.abp.Web.Host"]
COPY ["src/antflow.abp.Web.Core", "src/antflow.abp.Web.Core"]
COPY ["src/antflow.abp.Application", "src/antflow.abp.Application"]
COPY ["src/antflow.abp.Core", "src/antflow.abp.Core"]
COPY ["src/antflow.abp.EntityFrameworkCore", "src/antflow.abp.EntityFrameworkCore"]
WORKDIR "/src/src/antflow.abp.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "antflow.abp.Web.Host.dll"]
