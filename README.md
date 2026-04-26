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

## Características

- Registro de usuarios
- Login con JWT
- Refresh Tokens (7 días de validez)
- Access Tokens (15 minutos de validez)
- Autorización por roles
- Password hashing con HMACSHA512
- Validación de entrada con FluentValidation
- Seeder automático de usuario admin
- Endpoints protegidos con `[Authorize]`

## Arquitectura

```
Clean Architecture
├── Domain/           (Entidades)
├── Application/      (Lógica de negocio, DTOs, Interfaces)
├── Infrastructure/   (DbContext, Repository, UnitOfWork)
└── Presentation/    (Controllers)
```

## Patrones de Diseño

- **Repository Pattern**: Acceso a datos genérico con caché
- **Unit of Work**: Transaccionescentralizadas
- **Dependency Injection**: Inyección de dependencias
- **DTOs**: Separaciónrequest/response

## Estructura de Archivos

```
Server/
├── Domain/
│   └── Entities/
│       ├── User.cs
│       └── RefreshToken.cs
├── Application/
│   ├── DTOs/
│   │   └── AuthDtos.cs
│   ├── Validators/
│   │   └── AuthValidators.cs
│   ├── Interfaces/
│   │   └── IUnitOfWork.cs
│   └── Services/
│       └── AuthService.cs
├── Infrastructure/
│   └── Persistence/
│       ├── ApplicationDbContext.cs
│       ├── Repository.cs
│       ├── UnitOfWork.cs
│       └── DatabaseSeeder.cs
├── Presentation/
│   └── Controllers/
│       └── AuthController.cs
├── Program.cs
└── Server.csproj
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

## Credenciales por Defecto

```json
Email: admin@example.com
Password: Admin123!
```

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

## Comandos

```bash
# Restaurar dependencias
dotnet restore

# Compilar
dotnet build

# Correr proyecto
dotnet run

# Crear migración
dotnet ef migrations add InitialCreate

# Actualizar base de datos
dotnet ef database update
```

## Migraiones

Las migraciones se almacenan en `Server/Infrastructure/Persistence/Migrations/`.

## Mejoras Futuras

### Alta Prioridad
- [ ] Implementar CQRS con MediatR
- [ ] Agregar logging centralizado (Serilog)
- [ ] Error handling global con middleware
- [ ] Rate limiting

### Media Prioridad
- [ ] Tests unitarios (xUnit/NUnit)
- [ ] Implementar Refresh Token rotación
- [ ] Email validation
- [ ] Two-Factor Authentication (2FA)
- [ ] Password reset por email

### Baja Prioridad
- [ ] Docker support
- [ ] Health checks
- [ ] API versioning
- [ ] Cache con Redis
- [ ] Role-based authorization con permisos

## Licencia

MIT