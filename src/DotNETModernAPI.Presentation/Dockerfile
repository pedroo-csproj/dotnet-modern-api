FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DotNETModernAPI.Presentation/DotNETModernAPI.Presentation.csproj", "DotNETModernAPI.Presentation/"]
COPY ["DotNETModernAPI.Application/DotNETModernAPI.Application.csproj", "DotNETModernAPI.Application/"]
COPY ["DotNETModernAPI.Domain/DotNETModernAPI.Domain.csproj", "DotNETModernAPI.Domain/"]
COPY ["DotNETModernAPI.Infrastructure.CrossCutting.Core/DotNETModernAPI.Infrastructure.CrossCutting.Core.csproj", "DotNETModernAPI.Infrastructure.CrossCutting.Core/"]
COPY ["DotNETModernAPI.Infrastructure.CrossCutting/DotNETModernAPI.Infrastructure.CrossCutting.csproj", "DotNETModernAPI.Infrastructure.CrossCutting/"]
COPY ["DotNETModernAPI.Infrastructure.Data/DotNETModernAPI.Infrastructure.Data.csproj", "DotNETModernAPI.Infrastructure.Data/"]
COPY ["DotNETModernAPI.Infrastructure.Data.Migrations/DotNETModernAPI.Infrastructure.Data.Migrations.csproj", "DotNETModernAPI.Infrastructure.Data.Migrations/"]
RUN dotnet restore "DotNETModernAPI.Presentation/DotNETModernAPI.Presentation.csproj"
COPY . .
WORKDIR "/src/DotNETModernAPI.Presentation"
RUN dotnet build "DotNETModernAPI.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotNETModernAPI.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotNETModernAPI.Presentation.dll"]
