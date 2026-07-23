# Proyecto Gestión de Tareas

Esta es una aplicación web de gestión de tareas construida con **ASP.NET Core 10 (Razor Pages)**, **C# 14**, y **Entity Framework Core 10**, respaldada por una base de datos **SQLite**.

El proyecto se diseñó siguiendo estrictas pautas de arquitectura, rendimiento y características modernas del lenguaje.

## Funcionalidades Implementadas

1. **Gestión de Tareas**:
   - Listado de todas las tareas (ordenadas por fecha de creación desc).
   - Creación de nuevas tareas con validaciones en el frontend y backend.
   - Posibilidad de marcar una tarea como "Pendiente" o "Completada".
   - Eliminación de tareas.

2. **Modelo y Base de Datos**:
   - Única entidad `Tarea` con propiedades: `Id`, `Descripcion`, `EstaAcabada`, y `Fecha`.
   - `AppDbContext` configurado para SQLite con persistencia en `app.db`.

3. **Arquitectura y Mejores Prácticas (C# 14 y EF Core 10)**:
   - Uso de **Constructores Primarios** en todas las clases y controladores de páginas.
   - Uso de `AsNoTracking()` para consultas de solo lectura con EF Core.
   - Uso de **ExecuteUpdateAsync** y **ExecuteDeleteAsync** para actualizaciones/borrados eficientes directamente en base de datos.
   - DTOs y Comandos inmutables modelados como `record`.
   - Soporte ubicuo para `CancellationToken` en operaciones asíncronas.
   - Las páginas delegan su lógica al servicio `ITareasService`.
   - UI que utiliza **Tag Helpers** nativos de ASP.NET Core (`asp-for`, `asp-page`, etc.).
   - Cumplimiento del patrón **PRG (Post-Redirect-Get)** y uso de `TempData` para la comunicación entre peticiones tras las mutaciones.

## Pasos Seguidos para Crear el Proyecto

A continuación se detalla la secuencia de comandos y pasos empleados en la creación de esta aplicación:

1. **Inicialización del Proyecto**:
   ```bash
   dotnet new razor -n Tareas -o .
   ```

2. **Instalación de Dependencias**:
   Instalación de Entity Framework Core para SQLite y herramientas de diseño para ejecutar migraciones.
   ```bash
   dotnet add package Microsoft.EntityFrameworkCore.Sqlite
   dotnet add package Microsoft.EntityFrameworkCore.Design
   ```

3. **Desarrollo del Dominio (Modelos)**:
   Se creó el modelo `Tarea` en la carpeta `Models`, junto a los registros inmutables `TareaDto` y `CreateTareaCommand`.

4. **Configuración de Datos (DbContext)**:
   Creación de `Data/AppDbContext.cs` mapeando la entidad `Tarea`.

5. **Lógica de Negocio (Servicios)**:
   Creación de la interfaz `ITareasService` y su implementación `TareasService`, inyectando el DbContext con constructor primario de C# 14.

6. **Inyección de Dependencias**:
   Modificación de `Program.cs` para registrar:
   - Cadena de conexión para SQLite (configurada en `appsettings.json`).
   - `AppDbContext` mediante `AddDbContextPool`.
   - Registro de servicios con `AddScoped<ITareasService, TareasService>()`.

7. **Desarrollo de la UI (Razor Pages)**:
   - Se sobrescribió `Index.cshtml` / `Index.cshtml.cs` para presentar la lista, alternar el estado y eliminar tareas.
   - Se creó `Create.cshtml` / `Create.cshtml.cs` para el formulario de creación.

8. **Creación de la Base de Datos (EF Migrations)**:
   ```bash
   dotnet build
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## Cómo Ejecutar el Proyecto

Asegúrate de estar en la carpeta raíz del proyecto y tener instalado el SDK de .NET 10.

```bash
dotnet run
```
La aplicación estará disponible típicamente en `https://localhost:5001` o `http://localhost:5000` (revisa la salida de la consola para confirmar la URL exacta).
