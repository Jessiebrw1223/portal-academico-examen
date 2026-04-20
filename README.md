# Portal Académico — Gestión de Cursos y Matrículas

Proyecto de examen parcial desarrollado con **ASP.NET Core MVC (.NET 8)**, **Identity**, **EF Core + SQLite**, **Razor Views**, **Session** y **Cache distribuido con Redis**.

## Stack
- ASP.NET Core MVC (.NET 8)
- ASP.NET Core Identity
- Entity Framework Core
- SQLite
- Razor Views
- Session
- Redis (`IDistributedCache` + Session)
- Render.com para despliegue

## Funcionalidades cubiertas

### Pregunta 1 — Bootstrap + Dominio
- Proyecto MVC con Identity.
- Configuración de EF Core con SQLite.
- Modelos `Curso` y `Matricula`.
- Restricciones implementadas:
  - `Creditos > 0`
  - `HorarioInicio < HorarioFin`
  - `Curso.Codigo` único
  - un usuario no puede matricularse dos veces al mismo curso
- Seed inicial:
  - 3 cursos activos
  - 1 usuario con rol `Coordinador`

### Pregunta 2 — Catálogo y filtros
- Catálogo de cursos activos.
- Filtros por:
  - nombre
  - rango de créditos
  - horario
- Vista detalle con botón **Inscribirse**.
- Validaciones server-side de créditos y horario.

### Pregunta 3 — Matrículas
- Inscripción crea matrícula en estado `Pendiente`.
- Validaciones server-side:
  - usuario autenticado
  - no exceder cupo máximo
  - no duplicar matrícula por curso
  - no solapar con otro curso matriculado
- Feedback claro en la misma vista.

### Pregunta 4 — Session y Redis
- Session:
  - guarda el último curso visitado
  - muestra enlace en layout: `Volver al curso {Nombre}`
- Cache distribuida:
  - lista de cursos activos cacheada por 60 segundos
  - invalidación cuando se crea, edita, activa o desactiva un curso

### Pregunta 5 — Panel Coordinador
- Rol `Coordinador`.
- `/Coordinador` protegido con `[Authorize(Roles="Coordinador")]`.
- CRUD básico de cursos:
  - crear
  - editar
  - desactivar / activar
- Lista de matrículas por curso.
- Cambios de estado:
  - confirmar
  - cancelar

### Pregunta 6 — Deploy en Render
- Variables de entorno documentadas.
- `ASPNETCORE_URLS=http://0.0.0.0:${PORT}` es una forma válida de configurar el puerto en ASP.NET Core, y Render usa la variable `PORT` en sus Web Services. Redis se integra en ASP.NET Core mediante `AddStackExchangeRedisCache`.

---

## Credenciales seed

### Coordinador
- **Email:** `coordinador@portal.local`
- **Password:** `Coord123`

---

## Estructura del proyecto

```bash
PortalAcademicoExamen/
├── Controllers/
├── Data/
├── Models/
├── Services/
├── ViewModels/
├── Views/
├── wwwroot/
├── Program.cs
├── appsettings.json
├── render.yaml
└── README.md
```

---

## Pasos para correr localmente

### 1. Restaurar paquetes
```bash
dotnet restore
```

### 2. Crear base y correr app
Este proyecto usa `Database.EnsureCreated()` para simplificar la ejecución inmediata del examen/demo.

```bash
dotnet run
```

### 3. Abrir en navegador
```bash
http://localhost:5164
```

---

## Migraciones EF Core

> Nota: el proyecto está preparado para EF Core con SQLite, pero en esta entrega se dejó `EnsureCreated()` para facilitar la puesta en marcha inmediata.
> Si tu docente exige migraciones explícitas, puedes generarlas localmente con el SDK instalado:

```bash
dotnet tool restore
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Si quieres usar migraciones explícitas, luego reemplaza:

```csharp
db.Database.EnsureCreated();
```

por:

```csharp
db.Database.Migrate();
```

---

## Variables de entorno

### Local / Render
```env
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:${PORT}
ConnectionStrings__DefaultConnection=Data Source=portalacademico.db
Redis__ConnectionString=TU_CADENA_REDIS
```

> Si no defines `Redis__ConnectionString`, la aplicación usa `DistributedMemoryCache` como fallback local.

---

## Deploy en Render

### Build Command
```bash
dotnet publish -c Release -o out
```

### Start Command
```bash
dotnet out/PortalAcademicoExamen.dll
```

### Variables mínimas
- `ASPNETCORE_ENVIRONMENT=Production`
- `ASPNETCORE_URLS=http://0.0.0.0:${PORT}`
- `ConnectionStrings__DefaultConnection=Data Source=portalacademico.db`
- `Redis__ConnectionString=<cadena redis>`

---

## Flujo Git sugerido por pregunta

```bash
git checkout -b feature/bootstrap-dominio
git checkout -b feature/catalogo-cursos
git checkout -b feature/matriculas
git checkout -b feature/sesion-redis
git checkout -b feature/panel-coordinador
git checkout -b deploy/render
```

Cada rama debe cerrar con su respectivo PR hacia `main`.

---

## Notas técnicas
- El índice único para `Curso.Codigo` y para `(CursoId, UsuarioId)` se modela con EF Core. EF Core soporta índices únicos en Fluent API y Data Annotations.
- Se agregaron restricciones adicionales de dominio tanto con validaciones server-side como con `HasCheckConstraint`.
- La lógica de matrícula está encapsulada en `MatriculaService`.
- La lógica de caché está encapsulada en `CursoCacheService`.
- La arquitectura es MVC clásica: **Controllers + Models + Views + ViewModels + Services + Data**.

---

## Observación honesta de esta entrega
Este ZIP fue armado completamente desde cero y con estructura profesional, pero **no pudo ser compilado dentro de este entorno de trabajo porque aquí no está instalado el SDK de .NET**. Por eso, al abrirlo en tu máquina debes ejecutar `dotnet restore` y `dotnet run`, y si deseas una versión estricta con migraciones, generarlas localmente con `dotnet ef`.
