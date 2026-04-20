FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ONGES.Donate/ONGES.Donate.Api/ONGES.Donate.Api.csproj ONGES.Donate/ONGES.Donate.Api/
COPY ONGES.Donate/ONGES.Donate.Application/ONGES.Donate.Application.csproj ONGES.Donate/ONGES.Donate.Application/
COPY ONGES.Donate/ONGES.Donate.Infrastructure/ONGES.Donate.Infrastructure.csproj ONGES.Donate/ONGES.Donate.Infrastructure/
COPY ONGES.Donate/ONGES.Donate.Domain/ONGES.Donate.Domain.csproj ONGES.Donate/ONGES.Donate.Domain/

RUN dotnet restore "ONGES.Donate/ONGES.Donate.Api/ONGES.Donate.Api.csproj"

COPY . .
WORKDIR /src/ONGES.Donate/ONGES.Donate.Api
RUN dotnet publish "ONGES.Donate.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "ONGES.Donate.Api.dll"]
