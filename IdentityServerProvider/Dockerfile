#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["IdentityServerProvider/IdentityServerProvider.csproj", "IdentityServerProvider/"]
RUN dotnet restore "IdentityServerProvider/IdentityServerProvider.csproj"
COPY . .
WORKDIR "/src/IdentityServerProvider"
RUN dotnet build "IdentityServerProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServerProvider.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServerProvider.dll"]