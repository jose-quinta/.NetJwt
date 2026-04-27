# .NET JWT Authentication API

ASP.NET Core 6 Web API con autenticación JWT y arquitetura limpia.

## Tecnologías

| Categoría | Tecnología |
|-----------|-------------|
| Framework | ASP.NET Core 6 |
| Autenticación | JWT (JSON Web Tokens) |
| ORM | Entity Framework Core 6 |
| Base de datos | SQL Server |
| Validación | FluentValidation 11.7.1 |
| Documentación API | Swashbuckle (Swagger) |
| Testing | xUnit + Moq |
| Cache | IMemoryCache |

## Características

- Registro de usuarios
- Login con JWT
- Refresh Tokens (7 días de validade)
- Access Tokens (15 minutos de validade)
- Autorización por roles y permisos
- Password hashing con HMACSHA512
- Validación de entrada con FluentValidation
- Seeder automático de usuario admin
- Endpoints protegidos con `[Authorize]`
- Manejo de errores global con middleware
- Logging configurado
- Health checks (`/health`)
- API Versioning
- Docker support
- Unit Tests con xUnit

## Arquitectura

```
Clean Architecture (Multi-Project)
├── Domain/           (Entidades, Enums)
├── Application/      (Lógica de negocio, DTOs, Interfaces)
├── Infrastructure/   (DbContext, Repository, UnitOfWork)
└── Api/            (Controllers, Program, Middleware, Filters)
```

## Patrones de Diseño

- **Repository Pattern**: Acceso a datos genérico con caché
- **Unit of Work**: Transacciones centralizadas
- **Dependency Injection**: Inyección de dependencias
- **DTOs**: Separación request/response
- **Middleware**: Manejo global de errores
- **Permission-based Authorization**: Permisos por rol en tokens JWT

## Estructura de Archivos

```
Server/
├── Api/
│   ├── Controllers/
│   │   └── AuthController.cs
│   ├── Middleware/
│   │   └── ErrorHandlingMiddleware.cs
│   ├── Extensions/
│   │   └── ApplicationBuilderExtensions.cs
│   ├── Filters/
│   │   └── PermissionAttribute.cs
│   ├── Program.cs
│   └── Api.csproj
├── Application/
│   ├── DTOs/
│   │   └── AuthDtos.cs
│   ├── Validators/
│   │   └── AuthValidators.cs
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IUnitOfWork.cs
│   │   ├── IRepository.cs
│   │   ├── IConfigService.cs
│   │   └── IUserContext.cs
│   └── Services/
│       └── AuthService.cs
├── Domain/
│   ├── Entities/
│   │   ├── User.cs
│   │   └── RefreshToken.cs
│   └── Enums/
│       └── Roles.cs
├── Infrastructure/
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── ApplicationDbContextFactory.cs
│   │   └── Seeders/
│   │       └── DatabaseSeeder.cs
│   └── Repositories/
│       ├── Repository.cs
│       ├── UnitOfWork.cs
│       ├── ConfigService.cs
│       └── UserContext.cs
└── Tests/
    ├── Services/
    │   └── AuthServiceTests.cs
    ├── Repositories/
    │   └── RepositoryTests.cs
    └── Tests.csproj
```

## Endpoints

| Método | Endpoint | Descripción | Autorizado |
|--------|----------|------------|-----------|
| POST | `/api/auth/register` | Registrar usuario | No |
| POST | `/api/auth/login` | Iniciar sesión | No |
| POST | `/api/auth/refreshToken` | Renovar token | No |
| POST | `/api/auth/logout` | Cerrar sesión | Sí |
| GET | `/api/auth/name` | Obtener nombre | Sí |
| GET | `/api/auth/role` | Obtener rol | Sí |
| GET | `/health` | Health check | No |

## Credenciales por Defecto

```json
Email: admin@example.com
Password: Admin123!
```

## Permisos por Rol

| Rol | Permisos |
|-----|----------|
| Admin | ReadUsers, WriteUsers, DeleteUsers, ReadProducts, WriteProducts, DeleteProducts, ManageRoles |
| Moderator | ReadUsers, WriteUsers, ReadProducts, WriteProducts, DeleteProducts |
| User | ReadProducts |

## Configuración

Variables de entorno en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;Trusted_Connection=true"
  },
  "AppSettings": {
    "Token": "TuClaveSecreta"
  }
}
```

## Docker

```bash
# Construir imagen
docker build -t dotnet-jwt-api .

# Correr con docker-compose
docker-compose up
```

## Comandos

```bash
# Compilar proyecto
dotnet build Server/Api/Api.csproj

# Compilar tests
dotnet build Server/Tests/Tests.csproj

# Correr tests
dotnet test Server/Tests/Tests.csproj

# Correr proyecto
dotnet run --project Server/Api/Api.csproj

# Crear migración
dotnet ef migrations add InitialCreate --project Server/Infrastructure/Infrastructure.csproj

# Actualizar base de datos
dotnet ef database update --project Server/Infrastructure/Infrastructure.csproj
```

## Mejoras Futuras

### Alta Prioridad
- [ ] Implementar CQRS con MediatR
- [ ] Agregar Serilog para logging más completo
- [ ] Implementar Refresh Token rotación
- [ ] Rate limiting

### Media Prioridad
- [ ] **Usar `IdentityUser` de Microsoft.AspNetCore.Identity** - Reemplazar la entidad User personalizada por IdentityUser para seguir los estándares de .NET y obtener características como: gestión de usuarios, roles, confirmaciones de email, reset de password, autenticación de dos factores, protección contra ataques, etc.
- [ ] Email validation
- [ ] Two-Factor Authentication (2FA)
- [ ] Password reset por email

### Baja Prioridad
- [ ] Docker support (completado)
- [ ] Health checks (completado)
- [ ] API versioning (completado)
- [ ] Cache con Redis
- [ ] Role-based authorization con permisos (completado)
- [ ] Unit tests (completado)

## Licencia

MIT