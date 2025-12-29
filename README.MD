# Client API - Sistema de Gestión de Clientes

API RESTful desarrollada con **ASP.NET Core 8.0** para la gestión de clientes, con integración de APIs externas (JSONPlaceholder).

---

## 📋 Tabla de Contenidos

- [Características](#características)
- [Tecnologías](#tecnologías)
- [Requisitos Previos](#requisitos-previos)
- [Instalación](#instalación)
- [Configuración](#configuración)
- [Ejecución](#ejecución)
- [Endpoints Disponibles](#endpoints-disponibles)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Arquitectura](#arquitectura)
- [Ejemplos de Uso](#ejemplos-de-uso)
- [Autor](#autor)

---

## ✨ Características

- ✅ **CRUD Completo** de clientes bancarios
- ✅ **Entity Framework Core** con MySQL
- ✅ **Arquitectura en Capas** (Controllers, Services, Data, DTOs)
- ✅ **Inyección de Dependencias**
- ✅ **Validaciones** con Data Annotations
- ✅ **Logging** integrado
- ✅ **CORS** configurado para frontend
- ✅ **Swagger/OpenAPI** para documentación interactiva
- ✅ **Consumo de APIs Externas**:
  - JSONPlaceholder (usuarios ficticios)
- ✅ **Manejo de Errores** centralizado
- ✅ **Async/Await** para operaciones asíncronas

---

## 🛠️ Tecnologías

- **Framework**: ASP.NET Core 8.0
- **Lenguaje**: C# 12
- **ORM**: Entity Framework Core 8.0
- **Base de Datos**: MySQL
- **Documentación**: Swagger/OpenAPI
- **Logging**: Microsoft.Extensions.Logging
- **HTTP Client**: HttpClientFactory

### Paquetes NuGet Principales

```xml
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

---

## 📦 Requisitos Previos

- [.NET SDK 8.0 o superior](https://dotnet.microsoft.com/download)
- Editor de código (VS Code)
- Git (opcional)

---

## 🚀 Instalación

### 1. Clonar el repositorio

```bash
git clone https://github.com/GAMR11/BankClientAPI.git
cd BankClientAPI
```

### 2. Restaurar dependencias

```bash
dotnet restore
```

### 3. La base de datos se creará automáticamente al ejecutar la aplicación

---

## ⚙️ Configuración

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=127.0.0.1;Database=bankclients;User=root;Password=;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "Urls": "http://localhost:5000"
}
```

### CORS

El API está configurado para aceptar peticiones desde:
- `http://localhost:4200` (Angular development server)

Para modificar, edita `Program.cs`:

```csharp
policy.WithOrigins("http://localhost:4200")
```

---

## ▶️ Ejecución

### Modo Desarrollo

```bash
dotnet run
```

La API estará disponible en:
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger

### Modo Producción

```bash
dotnet run --configuration Release
```

---

## 📡 Endpoints Disponibles

### 🧑‍💼 Clientes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/clients` | Obtener todos los clientes activos |
| `GET` | `/api/clients/{id}` | Obtener cliente por ID |
| `POST` | `/api/clients` | Crear nuevo cliente |
| `PUT` | `/api/clients/{id}` | Actualizar cliente |
| `DELETE` | `/api/clients/{id}` | Desactivar cliente (soft delete) |
| `GET` | `/api/clients/search?term={term}` | Buscar clientes por nombre o email |

### 👥 Usuarios Externos (JSONPlaceholder)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `GET` | `/api/externalusers` | Obtener usuarios de API externa |
| `GET` | `/api/externalusers/{id}` | Obtener usuario específico |

---

## 📁 Estructura del Proyecto

```
BankClientAPI/
│
├── Controllers/
│   ├── ClientsController.cs          # CRUD de clientes
│   ├── ExternalUsersController.cs    # API JSONPlaceholder
│
├── Data/
│   └── AppDbContext.cs               # Contexto de Entity Framework
│
├── DTOs/
│   └── ClientDTOs.cs                 # Data Transfer Objects
│       ├── CreateClientDto
│       ├── UpdateClientDto
│       ├── ClientResponseDto
│       ├── ExternalUserDto
│
├── Models/
│   └── Client.cs                     # Modelo de datos (Entidad)
│
├── Services/
│   └── ExternalApiService.cs         # Consumo de APIs externas
│       └── IExternalApiService       # Interface del servicio
│
├── Program.cs                        # Configuración principal
├── appsettings.json                  # Configuración de la app
```

---

## 🏗️ Arquitectura

### Patrón de Capas

```
┌─────────────────────────────────────┐
│         Controllers Layer           │  ← Maneja HTTP requests/responses
│    (ClientsController, etc.)        │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│          Services Layer             │  ← Lógica de negocio
│    (ExternalApiService)             │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│          Data Layer                 │  ← Acceso a datos
│    (AppDbContext, EF Core)          │
└─────────────────┬───────────────────┘
                  │
┌─────────────────▼───────────────────┐
│          Database                   │  ← MySQL
│                                     │ 
└─────────────────────────────────────┘
```

### Dependency Injection

Todos los servicios se registran en `Program.cs`:

```csharp
// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// External API Service
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();

// Controllers
builder.Services.AddControllers();
```

---

## 📝 Ejemplos de Uso

### 1. Crear un Cliente

**Request:**
```http
POST /api/clients
Content-Type: application/json

{
  "firstName": "Carlos",
  "lastName": "Rodríguez",
  "email": "carlos@email.com",
  "phoneNumber": "0987654321",
  "address": "Av. 6 de Diciembre, Quito",
  "accountType": "Ahorros",
  "balance": 5000.00
}
```

**Response:** `201 Created`
```json
{
  "id": 3,
  "firstName": "Carlos",
  "lastName": "Rodríguez",
  "fullName": "Carlos Rodríguez",
  "email": "carlos@email.com",
  "phoneNumber": "0987654321",
  "address": "Av. 6 de Diciembre, Quito",
  "accountType": "Ahorros",
  "balance": 5000.00,
  "createdAt": "2024-12-28T10:30:00Z",
  "isActive": true
}
```

### 2. Obtener Todos los Clientes

**Request:**
```http
GET /api/clients
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "firstName": "Juan",
    "lastName": "Pérez",
    "fullName": "Juan Pérez",
    "email": "juan.perez@email.com",
    "phoneNumber": "0999123456",
    "address": "Av. Amazonas y Naciones Unidas, Quito",
    "accountType": "Ahorros",
    "balance": 5000.00,
    "createdAt": "2024-12-28T08:00:00Z",
    "isActive": true
  },
  {
    "id": 2,
    "firstName": "María",
    "lastName": "González",
    "fullName": "María González",
    "email": "maria.gonzalez@email.com",
    "phoneNumber": "0998765432",
    "address": "Calle 10 de Agosto, Quito",
    "accountType": "Corriente",
    "balance": 12500.50,
    "createdAt": "2024-12-28T08:00:00Z",
    "isActive": true
  }
]
```

### 3. Buscar Clientes

**Request:**
```http
GET /api/clients/search?term=juan
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "firstName": "Juan",
    "lastName": "Pérez",
    "fullName": "Juan Pérez",
    "email": "juan.perez@email.com",
    ...
  }
]
```

### 4. Actualizar Cliente

**Request:**
```http
PUT /api/clients/1
Content-Type: application/json

{
  "balance": 6000.00,
  "phoneNumber": "0999999999"
}
```

**Response:** `204 No Content`

### 5. Obtener Usuarios Externos

**Request:**
```http
GET /api/externalusers
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Leanne Graham",
    "email": "Sincere@april.biz",
    "phone": "1-770-736-8031 x56442",
    "address": {
      "street": "Kulas Light",
      "city": "Gwenborough"
    }
  },
  ...
]
```

---

## 🔒 Validaciones

### Modelo Client

- `FirstName`: Requerido, máximo 100 caracteres
- `LastName`: Requerido, máximo 100 caracteres
- `Email`: Requerido, formato válido, único en BD
- `PhoneNumber`: Requerido, formato válido
- `AccountType`: Requerido
- `Balance`: Requerido, debe ser ≥ 0

### Manejo de Errores

| Código | Descripción |
|--------|-------------|
| `200 OK` | Operación exitosa |
| `201 Created` | Recurso creado exitosamente |
| `204 No Content` | Actualización/eliminación exitosa |
| `400 Bad Request` | Validación fallida o datos incorrectos |
| `404 Not Found` | Recurso no encontrado |
| `502 Bad Gateway` | Error al conectar con API externa |

---

## 🗄️ Base de Datos

### Modelo de Datos

**Tabla: Clients**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | int | Primary Key (auto-incremental) |
| FirstName | string(100) | Nombre del cliente |
| LastName | string(100) | Apellido del cliente |
| Email | string(150) | Email único |
| PhoneNumber | string(20) | Teléfono |
| Address | string(200) | Dirección (opcional) |
| AccountType | string(50) | Tipo de cuenta |
| Balance | decimal(18,2) | Saldo de la cuenta |
| CreatedAt | datetime | Fecha de creación |
| IsActive | bool | Estado del cliente |

### Datos Iniciales (Seed Data)

El sistema incluye 2 clientes de prueba:
1. Juan Pérez - Cuenta de Ahorros
2. María González - Cuenta Corriente

---

## 🧪 Testing

### Pruebas con Swagger

1. Ejecuta la aplicación: `dotnet run`
2. Abre: http://localhost:5000/swagger
3. Prueba cada endpoint desde la interfaz interactiva

### Pruebas con cURL

```bash
# Obtener todos los clientes
curl http://localhost:5000/api/clients

# Crear cliente
curl -X POST http://localhost:5000/api/clients \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Test",
    "lastName": "User",
    "email": "test@email.com",
    "phoneNumber": "0999999999",
    "accountType": "Ahorros",
    "balance": 1000.00
  }'

# Obtener Pokémon
curl http://localhost:5000/api/pokemons/ditto
```

---

## 🚀 Despliegue

### Publicar para Producción

```bash
dotnet publish -c Release -o ./publish
```

### Docker (Opcional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish .
EXPOSE 5000
ENTRYPOINT ["dotnet", "BankClientAPI.dll"]
```

---

## 📚 Conceptos Clave Implementados

- ✅ **RESTful API Design**
- ✅ **Dependency Injection (DI)**
- ✅ **Entity Framework Core (ORM)**
- ✅ **Async/Await Pattern**
- ✅ **DTO Pattern**
- ✅ **Repository Pattern** (via DbContext)
- ✅ **CORS Configuration**
- ✅ **Error Handling**
- ✅ **Logging**
- ✅ **Data Validation**
- ✅ **HTTP Client Best Practices**

---

## 📖 Recursos de Aprendizaje

- [Documentación oficial de ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [C# Documentation](https://docs.microsoft.com/dotnet/csharp)
- [RESTful API Design](https://restfulapi.net)

---

## 👨‍💻 Autor

**[Gustavo Morales](https://github.com/GAMR11)**

- GitHub: [@GAMR11](https://github.com/GAMR11)
- LinkedIn: [Tu Perfil](https://linkedin.com/in/gustavo-morales-640259221)
- Email: gamr130898@gmail.com

---

## 📌 Notas Adicionales

### Próximas Mejoras

- [ ] Implementar autenticación JWT
- [ ] Agregar paginación en GET /api/clients
- [ ] Implementar filtros avanzados
- [ ] Agregar Unit Tests
- [ ] Implementar caché con Redis
- [ ] Agregar rate limiting
- [ ] Dockerizar la aplicación

### Changelog

**v1.0.0** (2025-12-28)
- ✨ Release inicial
- ✅ CRUD completo de clientes
- ✅ Integración con APIs externas
- ✅ Documentación con Swagger

---

⭐ Si este proyecto te fue útil, dale una estrella en GitHub!
