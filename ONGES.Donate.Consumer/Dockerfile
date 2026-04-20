FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ONGES.Donate/ONGES.Donate.Consumer/ONGES.Donate.Consumer.csproj ONGES.Donate/ONGES.Donate.Consumer/
COPY ONGES.Donate/ONGES.Donate.Application/ONGES.Donate.Application.csproj ONGES.Donate/ONGES.Donate.Application/
COPY ONGES.Donate/ONGES.Donate.Infrastructure/ONGES.Donate.Infrastructure.csproj ONGES.Donate/ONGES.Donate.Infrastructure/
COPY ONGES.Donate/ONGES.Donate.Domain/ONGES.Donate.Domain.csproj ONGES.Donate/ONGES.Donate.Domain/

RUN dotnet restore "ONGES.Donate/ONGES.Donate.Consumer/ONGES.Donate.Consumer.csproj"

COPY . .
WORKDIR /src/ONGES.Donate/ONGES.Donate.Consumer
RUN dotnet publish "ONGES.Donate.Consumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ONGES.Donate.Consumer.dll"]
