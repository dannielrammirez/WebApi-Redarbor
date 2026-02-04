# Redarbor Employee Management API

Web API REST profesional para la gestión de empleados, construida con **.NET 10**, siguiendo principios de **Clean Architecture**, **DDD**, **CQRS** y **SOLID**.

## Arquitectura

La solución implementa una arquitectura en capas siguiendo los principios de Clean Architecture:

```
┌─────────────────────────────────────────────────────────────┐
│                      Redarbor.API                           │
│  (Controllers, Middleware, Filters)                         │
├─────────────────────────────────────────────────────────────┤
│                   Redarbor.Application                      │
│  (CQRS: Commands/Queries, DTOs, Validators, MediatR)        │
├─────────────────────────────────────────────────────────────┤
│                  Redarbor.Infrastructure                    │
│  (EF Core Write, Dapper Read, JWT, Repositories)            │
├─────────────────────────────────────────────────────────────┤
│                     Redarbor.Domain                         │
│  (Entities, Value Objects, Interfaces, Domain Events)       │
└─────────────────────────────────────────────────────────────┘
```

### Justificación de Decisiones Técnicas

| Decisión | Justificación |
|----------|---------------|
| **Clean Architecture** | Separación de responsabilidades, testabilidad, independencia de frameworks |
| **DDD** | Modelado del dominio con entidades ricas, Value Objects para invariantes |
| **CQRS** | Separación de lectura/escritura para optimización y escalabilidad |
| **EF Core (Escritura)** | ORM maduro con tracking de cambios, Unit of Work integrado |
| **Dapper (Lectura)** | Rendimiento óptimo para consultas de solo lectura |
| **MediatR** | Desacoplamiento de handlers, pipeline behaviors (validación, logging) |
| **FluentValidation** | Validaciones declarativas, testables y mantenibles |
| **JWT Bearer** | Estándar de la industria para autenticación stateless en APIs |

## Tecnologías

- **.NET 10** - Framework principal
- **Entity Framework Core 10** - ORM para operaciones de escritura
- **Dapper** - Micro-ORM para operaciones de lectura
- **MediatR** - Implementación del patrón Mediator (CQRS)
- **FluentValidation** - Validación de comandos y queries
- **Serilog** - Logging estructurado
- **JWT Bearer** - Autenticación y autorización
- **SQL Server** - Base de datos
- **Docker** - Containerización
- **xUnit** - Framework de testing
- **Moq** - Mocking para tests
- **FluentAssertions** - Assertions expresivas

## Estructura del Proyecto

```
WebApi-Redarbor/
├── src/
│   ├── Redarbor.Domain/           # Entidades, Value Objects, Interfaces
│   ├── Redarbor.Application/      # CQRS, DTOs, Validators
│   ├── Redarbor.Infrastructure/   # EF Core, Dapper, JWT
│   └── Redarbor.API/              # Controllers, Middleware
├── tests/
│   ├── Redarbor.Domain.Tests/
│   ├── Redarbor.Application.Tests/
│   └── Redarbor.API.Tests/
├── scripts/
│   └── init-db.sql
├── docker-compose.yml
├── Dockerfile
└── README.md
```

## Requisitos Previos

- .NET 10 SDK
- SQL Server (LocalDB o instancia completa)

## Inicio Rápido con Docker

```bash
# Clonar el repositorio
git clone <repository-url>
cd WebApi-Redarbor

# Iniciar con Docker Compose
docker-compose up -d

# La API estará disponible en:
# http://localhost:5000
# Swagger UI: http://localhost:5000
```

## Desarrollo Local

```bash
# Restaurar paquetes
dotnet restore

# Ejecutar SQL Server (si no usas Docker)
# Asegúrate de tener SQL Server corriendo y actualiza la cadena de conexión

# Ejecutar migraciones
dotnet ef database update -p src/Redarbor.Infrastructure -s src/Redarbor.API

# Ejecutar la API
dotnet run --project src/Redarbor.API

# Ejecutar tests
dotnet test
```

## API Endpoints

### Autenticación

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/oauth2/token` | Obtener token JWT (OAuth2) |

**Request:**
```json
{
  "username": "admin",
  "password": "Admin@123456"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2024-01-01T12:00:00Z",
  "employeeId": 1,
  "username": "admin"
}
```

### Empleados

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/redarbor` | Obtener todos los empleados |
| GET | `/api/redarbor/{id}` | Obtener empleado por ID |
| POST | `/api/redarbor` | Crear nuevo empleado |
| PUT | `/api/redarbor/{id}` | Actualizar empleado |
| DELETE | `/api/redarbor/{id}` | Eliminar empleado (soft delete) |

> **Nota:** Todos los endpoints de empleados requieren autenticación JWT.

### Crear Empleado

**POST /api/redarbor**

```json
{
  "companyId": 1,
  "email": "john.doe@example.com",
  "password": "SecurePassword123",
  "portalId": 1,
  "roleId": 1,
  "statusId": 1,
  "username": "johndoe",
  "name": "John Doe",
  "telephone": "+1234567890",
  "fax": "+1234567891"
}
```

### Actualizar Empleado

**PUT /api/redarbor/{id}**

```json
{
  "username": "test1updated",
  "name": "John Doe Updated",
  "email": "john.updated@example.com"
}
```

## Flujo Funcional Completo

1. **Autenticación (OAuth2):**
   ```bash
   curl -X POST http://localhost:5000/api/oauth2/token \
     -H "Content-Type: application/x-www-form-urlencoded" \
     -d "grant_type=password&username=admin&password=Admin@123456"
   ```

2. **Crear Empleado:**
   ```bash
   curl -X POST http://localhost:5000/api/redarbor \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer <token>" \
     -d '{"companyId":1,"email":"test@example.com","password":"Test@123","portalId":1,"roleId":1,"statusId":1,"username":"test1"}'
   ```

3. **Consultar Todos:**
   ```bash
   curl http://localhost:5000/api/redarbor \
     -H "Authorization: Bearer <token>"
   ```

4. **Consultar por ID:**
   ```bash
   curl http://localhost:5000/api/redarbor/1 \
     -H "Authorization: Bearer <token>"
   ```

5. **Actualizar:**
   ```bash
   curl -X PUT http://localhost:5000/api/redarbor/1 \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer <token>" \
     -d '{"username":"test1updated"}'
   ```

6. **Eliminar:**
   ```bash
   curl -X DELETE http://localhost:5000/api/redarbor/1 \
     -H "Authorization: Bearer <token>"
   ```

## Seguridad

### Contraseñas
- Hasheadas con **PBKDF2-SHA256** (100,000 iteraciones)
- Salt aleatorio de 16 bytes por contraseña
- Verificación de tiempo constante

### JWT
- Algoritmo: HS256
- Expiración configurable (default: 60 minutos)
- Validación de issuer, audience y lifetime

### Validaciones
- Todas las entradas son validadas con FluentValidation
- Sanitización de datos en Value Objects
- Protección contra inyección SQL (parametrización)

## Tests

```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Tests específicos
dotnet test --filter "FullyQualifiedName~Domain.Tests"
dotnet test --filter "FullyQualifiedName~Application.Tests"
dotnet test --filter "FullyQualifiedName~API.Tests"
```

## Configuración

### Variables de Entorno

| Variable | Descripción | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | Cadena de conexión SQL Server | - |
| `JwtSettings__SecretKey` | Clave secreta JWT (min 32 chars) | - |
| `JwtSettings__Issuer` | Emisor del token | Redarbor.API |
| `JwtSettings__Audience` | Audiencia del token | Redarbor.Client |
| `JwtSettings__ExpirationInMinutes` | Tiempo de expiración | 60 |

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RedarborDb;..."
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!",
    "Issuer": "Redarbor.API",
    "Audience": "Redarbor.Client",
    "ExpirationInMinutes": 60
  }
}
```

## Health Checks

- **Endpoint:** `GET /health`
- Verifica conectividad con la base de datos

## Logging

Logs estructurados con Serilog:
- **Consola:** Formato legible para desarrollo
- **Archivo:** `logs/redarbor-YYYYMMDD.log` (rotación diaria)

## Códigos HTTP

| Código | Descripción |
|--------|-------------|
| 200 | Operación exitosa |
| 201 | Recurso creado |
| 204 | Operación exitosa sin contenido |
| 400 | Error de validación |
| 401 | No autenticado |
| 404 | Recurso no encontrado |
| 409 | Conflicto (duplicado) |
| 500 | Error interno |

## Licencia

Este proyecto es una prueba técnica y no tiene licencia comercial.
