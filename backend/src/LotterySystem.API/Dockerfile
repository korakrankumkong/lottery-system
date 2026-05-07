FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/src/LotterySystem.API/LotterySystem.API.csproj backend/src/LotterySystem.API/
COPY backend/src/LotterySystem.Application/LotterySystem.Application.csproj backend/src/LotterySystem.Application/
COPY backend/src/LotterySystem.Infrastructure/LotterySystem.Infrastructure.csproj backend/src/LotterySystem.Infrastructure/
COPY backend/src/LotterySystem.Domain/LotterySystem.Domain.csproj backend/src/LotterySystem.Domain/
RUN dotnet restore backend/src/LotterySystem.API/LotterySystem.API.csproj

COPY backend/src backend/src
RUN dotnet publish backend/src/LotterySystem.API/LotterySystem.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "LotterySystem.API.dll"]
