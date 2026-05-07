FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY LotterySystem.API.csproj ./
COPY ../LotterySystem.Application/LotterySystem.Application.csproj ../LotterySystem.Application/
COPY ../LotterySystem.Infrastructure/LotterySystem.Infrastructure.csproj ../LotterySystem.Infrastructure/
COPY ../LotterySystem.Domain/LotterySystem.Domain.csproj ../LotterySystem.Domain/
RUN dotnet restore LotterySystem.API.csproj

COPY . ./
COPY ../LotterySystem.Application ../LotterySystem.Application
COPY ../LotterySystem.Infrastructure ../LotterySystem.Infrastructure
COPY ../LotterySystem.Domain ../LotterySystem.Domain
RUN dotnet publish LotterySystem.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "LotterySystem.API.dll"]
