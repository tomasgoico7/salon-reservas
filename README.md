# Salon Reservas

API REST y SPA para gestionar reservas de salones de eventos infantiles.

## Stack

**Backend:** ASP.NET Core 8, EF Core InMemory, FluentValidation, xUnit + Moq + FluentAssertions  
**Frontend:** React 18, TypeScript, Vite, Material UI, React Hook Form + Zod, Axios  
**Infra:** Docker, docker-compose, Nginx

## Correr el proyecto

### Sin Docker

Backend (requiere .NET 8 SDK):
```bash
cd backend
dotnet run --project src/RoomReservations.Api
```
API en `http://localhost:5080`, Swagger en `http://localhost:5080/swagger`.

Frontend (requiere Node 20+):
```bash
cd frontend
npm install
npm run dev
```
App en `http://localhost:5173`. Hace proxy automático a la API local.

### Con Docker

```bash
docker compose up --build
```

- App: `http://localhost:8080`
- API: `http://localhost:5080`
- Swagger: `http://localhost:5080/swagger`

```bash
docker compose down
```

## Tests

### Backend

```bash
cd backend
dotnet test
```

65 tests distribuidos en tres proyectos:
- `RoomReservations.Domain.Tests` — invariantes de entidades y lógica de conflictos
- `RoomReservations.Application.Tests` — servicios con repositorios mockeados y validadores
- `RoomReservations.Api.Tests` — middleware de excepciones + integración E2E con `WebApplicationFactory`

### Frontend

```bash
cd frontend
npm run test:coverage
```

21 tests con Vitest + React Testing Library, 100% de cobertura en los módulos testeados:
- `isApiError.test.ts` — type guard de respuestas de error de la API
- `reservationSchema.test.ts` — validaciones del esquema Zod del formulario
- `ErrorBoundary.test.tsx` — comportamiento del componente de error boundary

## API

### GET /api/rooms

```json
[
  { "id": 1, "name": "Salon Estrella", "maxCapacity": 80, "isActive": true },
  { "id": 2, "name": "Salon Aventura", "maxCapacity": 60, "isActive": true },
  { "id": 3, "name": "Salon Magico",   "maxCapacity": 100, "isActive": true },
  { "id": 4, "name": "Salon Galaxia",  "maxCapacity": 50, "isActive": true }
]
```

### POST /api/reservations

```json
{
  "roomId": 1,
  "customerName": "Maria Lopez",
  "eventName": "Cumpleanios de Sofia",
  "guestCount": 25,
  "date": "2026-06-15",
  "startTime": "10:00:00",
  "endTime": "12:00:00"
}
```

Devuelve `201 Created` con la reserva completa. Errores posibles:
- `400` — validación de campos o reglas de horario
- `404` — salón inexistente
- `409` — solapamiento con reserva existente o buffer de 30 minutos no respetado

Respuesta de error estándar:
```json
{
  "status": 409,
  "title": "Conflicto de reserva",
  "detail": "La reserva se superpone con otra existente o no respeta el intervalo minimo de 30 minutos.",
  "timestamp": "2026-06-15T14:00:00Z"
}
```

Errores de validación incluyen un diccionario `errors` con los campos afectados.

### GET /api/reservations?date=2026-06-15

Lista de reservas para la fecha indicada, ordenadas por salón y hora de inicio.

## Arquitectura

Clean Architecture en cuatro capas con dependencias apuntando hacia adentro:

```
Api  →  Application  →  Domain
Infrastructure  →  Domain
```

Las reglas de negocio viven en el dominio: la entidad `Reservation` valida invariantes en el constructor y determina conflictos entre reservas via `ConflictsWith()`. `BusinessRules` centraliza las constantes configurables (09:00–18:00, buffer de 30 minutos). El `ExceptionHandlingMiddleware` traduce excepciones de dominio a respuestas HTTP consistentes.

EF Core InMemory simplifica el setup; cambiar a PostgreSQL o SQL Server requiere únicamente modificar `InfrastructureServiceCollectionExtensions`.
