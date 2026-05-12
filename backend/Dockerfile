# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY RoomReservations.sln ./
COPY src/RoomReservations.Domain/RoomReservations.Domain.csproj                 src/RoomReservations.Domain/
COPY src/RoomReservations.Application/RoomReservations.Application.csproj       src/RoomReservations.Application/
COPY src/RoomReservations.Infrastructure/RoomReservations.Infrastructure.csproj src/RoomReservations.Infrastructure/
COPY src/RoomReservations.Api/RoomReservations.Api.csproj                       src/RoomReservations.Api/

RUN dotnet restore src/RoomReservations.Api/RoomReservations.Api.csproj

COPY src/ src/
RUN dotnet publish src/RoomReservations.Api/RoomReservations.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# La imagen aspnet:8.0 ya trae el usuario 'app' (uid 1654) por defecto
USER app

COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "RoomReservations.Api.dll"]
