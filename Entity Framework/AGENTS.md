# `.gemini.md` — Guía de Arquitectura y Estándares de Código para Agentes IA
**Proyecto:** ASP.NET Core Web Application (.NET 10 / C# 14 / Entity Framework Core / Razor Pages)  
**Versión del Estándar:** 2026.1 (LTS Stack)  
**Objetivo:** Instrucciones normativas, mejores prácticas y patrones arquitectónicos para Gemini y agentes de codificación asistida en este repositorio.

---

## 🤖 1. Rol y Comportamiento del Agente IA

Cuando actúes como asistente, generador de código o revisor en este repositorio, DEBES adherirte estrictamente a los siguientes principios estructurales y estilísticos:

1. **Pila Tecnológica Moderna y Estricta:**  
   - Todo el código debe escribirse apuntando a **.NET 10** y utilizar la sintaxis moderna de **C# 14**.  
   - No generes código heredado (legacy), construcciones arcaicas ni boilerplate innecesario.
2. **Prioridad a la Inmutabilidad y Rendimiento:**  
   - Prioriza estructuras inmutables (`record`, `readonly struct`, `IReadOnlyCollection<T>`) para datos en circulación (DTOs, comandos, consultas).  
   - Minimiza la asignación de memoria en el montón (heap allocations) utilizando `Span<T>`, `ReadOnlySpan<T>` y colecciones expresivas.
3. **Principio de Responsabilidad Única (SRP) en Páginas Web:**  
   - Las **Razor Pages** son exclusivas para la capa de presentación y orquestación HTTP.  
   - Está **estrictamente prohibido** incrustar lógica de negocio compleja o acceso directo sin proyectar en los manejadores (`PageModel`). Utiliza servicios de aplicación o consultas proyectadas.
4. **Seguridad y Resiliencia por Defecto:**  
   - Todo método asíncrono debe aceptar y propagar un `CancellationToken`.  
   - Las consultas de solo lectura en Entity Framework Core deben ser **siempre** sin seguimiento (`AsNoTracking()`).

---

## ⚡ 2. Prácticas Modernas de C# 14 (.NET 10)

El código generado en este repositorio debe explotar las características más recientes del lenguaje C# para maximizar la legibilidad, la expresividad y la seguridad de tipos.

### 2.1. Constructores Primarios (Primary Constructors)
Úsalos en todas las clases, servicios y `PageModel` donde los parámetros se utilicen principalmente para inyección de dependencias o inicialización, eliminando campos privados repetitivos y constructores tradicionales.

```csharp
// ❌ INCORRECTO (Estilo Legacy - No usar en este proyecto)
public class OrderService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<OrderService> _logger;

    public OrderService(ApplicationDbContext db, ILogger<OrderService> logger)
    {
        _db = db;
        _logger = logger;
    }
}

// ✅ CORRECTO (Estilo C# 14 con Constructores Primarios)
public class OrderService(ApplicationDbContext db, ILogger<OrderService> logger)
{
    public async Task<OrderDto?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        logger.LogInformation("Recuperando orden con ID: {OrderId}", orderId);
        return await db.Orders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(o => new OrderDto(o.Id, o.CustomerName, o.TotalAmount, o.Status))
            .FirstOrDefaultAsync(ct);
    }
}
```

### 2.2. Propiedades con Campo Respaldado (Keyword `field`)
C# 14 introduce la palabra clave contextual `field`, que permite acceder directamente al campo de respaldo generado por el compilador en propiedades personalizadas. **No declares campos privados explícitos (`private string _message;`) solo para respaldar una propiedad con validación o formateo.**

```csharp
// ❌ INCORRECTO (Sintaxis antigua con backing field manual)
private string _customerEmail = string.Empty;
public string CustomerEmail
{
    get => _customerEmail;
    set => _customerEmail = value?.Trim().ToLowerInvariant() ?? throw new ArgumentNullException(nameof(value));
}

// ✅ CORRECTO (C# 14 usando la palabra clave 'field')
public string CustomerEmail
{
    get;
    set => field = value?.Trim().ToLowerInvariant() 
                   ?? throw new ArgumentNullException(nameof(value));
}

// Ejemplo de propiedad reactiva o calculada simple con backing field autogenerado
public decimal DiscountedPrice
{
    get;
    set => field = value < 0 ? 0 : value;
}
```

### 2.3. Uso de Records para Modelos de Datos, DTOs y CQRS
Utiliza `record` o `readonly record struct` para cualquier objeto que represente una transferencia de datos, modelos de vista de solo lectura, o parámetros de comando/consulta. Esto garantiza igualdad estructural y sintaxis concisa (`with`).

```csharp
// DTOs posicionales inmutables usando records
public record CustomerSummaryDto(Guid Id, string FullName, string Email, int TotalOrders);

// Record para actualización posicional con validación en parámetros
public record UpdateCustomerCommand(
    Guid CustomerId, 
    string FullName, 
    string PhoneNumber)
{
    public bool IsValid() => !string.IsNullOrWhiteSpace(FullName) && CustomerId != Guid.Empty;
}

// Para colecciones de alto rendimiento sin asignación de heap si es posible
public readonly record struct PricePoint(DateTime Timestamp, decimal Value);
```

### 2.4. Expresiones de Colección y Patrones Primitivos
Utiliza la sintaxis de expresiones de colección (`[...]`) para arreglos, listas y tramos (`Span<T>`), y aprovecha el emparejamiento de patrones avanzado.

```csharp
// ✅ Expresiones de colección modernas
int[] allowedStatuses = [1, 2, 5, 8];
List<string> defaultRoles = ["User", "Viewer"];
ReadOnlySpan<byte> magicHeader = [0x47, 0x49, 0x46]; // Conversión implícita a ReadOnlySpan (C# 14)

// ✅ Emparejamiento de patrones relacionales y lógicos
public string GetOrderUrgency(int hoursPending, decimal amount) => (hoursPending, amount) switch
{
    ( > 48, >= 1000m) => "Critical",
    ( > 24, _) or (_, >= 5000m) => "High",
    ( <= 4, < 100m) => "Low",
    _ => "Normal"
};
```

### 2.5. Genéricos No Vinculados con `nameof` y Modificadores en Lambdas
Aprovecha `nameof` con tipos genéricos no vinculados para un registro y diagnóstico limpios sin necesidad de especificar tipos ficticios.

```csharp
// ✅ C# 14: nameof en genéricos abiertos
logger.LogWarning("Fallo en la caché para el repositorio {RepoType}", nameof(IRepository<>));
string dictionaryTypeName = nameof(Dictionary<,>);

// ✅ Modificadores de parámetros (ref, in, out, scoped) en expresiones lambda
var parser = (string input, out int result) => int.TryParse(input, out result);
```

---

## 🗄️ 3. Entity Framework Core (v9 / v10) — Prácticas de Rendimiento y Arquitectura

Entity Framework Core en su versión moderna es un ORM extremadamente rápido si se configura y utiliza adecuadamente. El código generado debe seguir estas directrices sin excepción:

### 3.1. Consultas sin Seguimiento (`AsNoTracking`) por Defecto
Cualquier consulta destinada exclusivamente a lectura (como renderizar un listado en una Razor Page o un reporte) **DEBE** ejecutar `.AsNoTracking()` o `.AsNoTrackingWithIdentityResolution()`. Nunca cargues entidades completas al Change Tracker si no vas a modificarlas y llamar a `SaveChangesAsync()`.

```csharp
// ✅ CORRECTO: Consulta sin seguimiento con proyección a DTO (Record)
public async Task<IReadOnlyList<ProductCatalogDto>> GetActiveProductsAsync(
    Guid categoryId, 
    CancellationToken ct = default)
{
    return await db.Products
        .AsNoTracking()
        .Where(p => p.CategoryId == categoryId && p.IsActive)
        .OrderBy(p => p.Name)
        .Select(p => new ProductCatalogDto(p.Id, p.Name, p.Price, p.StockQuantity))
        .ToListAsync(ct);
}
```

### 3.2. Proyección Temprana (Select Projection over Entity Fetching)
Evita el anti-patrón *Over-fetching*. No recuperes entidades con 30 columnas y relaciones pesadas para mostrar 3 campos en la vista. Utiliza `.Select()` para que el motor SQL ejecute un `SELECT col1, col2` en lugar de `SELECT *`.

### 3.3. Operaciones Masivas (Bulk Operations)
Para actualizaciones o eliminaciones que afectan a múltiples filas, está **prohibido** cargar las entidades en memoria para modificarlas individualmente en un bucle. Utiliza los métodos modernos de EF Core `ExecuteUpdateAsync` y `ExecuteDeleteAsync`.

```csharp
// ❌ INCORRECTO: Carga innecesaria en memoria (Lento, alto consumo de RAM)
var oldLogs = await db.AuditLogs.Where(l => l.CreatedAt < cutoffDate).ToListAsync(ct);
db.AuditLogs.RemoveRange(oldLogs);
await db.SaveChangesAsync(ct);

// ✅ CORRECTO (EF Core 9/10): Eliminación directa en base de datos (1 sola consulta SQL)
int deletedRows = await db.AuditLogs
    .Where(l => l.CreatedAt < cutoffDate)
    .ExecuteDeleteAsync(ct);

// ✅ CORRECTO: Actualización masiva directa con lambdas modernas
int updatedRows = await db.Products
    .Where(p => p.CategoryId == targetCategoryId && p.Price < 100m)
    .ExecuteUpdateAsync(setters => setters
        .SetProperty(p => p.Price, p => p.Price * 1.10m)
        .SetProperty(p => p.LastModifiedAt, DateTime.UtcNow), ct);
```

### 3.4. Tipos Complejos (`[ComplexType]`) y Mapeo JSON
Para objetos de valor que no tienen identidad propia en la base de datos (como una Dirección, un Rango de Fechas o Metadatos), utiliza **Complex Types** de EF Core 9/10 o mapeo de columnas JSON (`.ToJson()`), evitando proliferación innecesaria de tablas relacionales.

```csharp
// Declaración de Tipo Complejo (Value Object)
[ComplexType]
public record Address(string Street, string City, string PostalCode, string Country);

public class Customer
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required Address BillingAddress { get; set; } // Almacenado como columnas planas o JSON
}

// Configuración en Fluent API (DbContext)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Customer>()
        .OwnsOne(c => c.BillingAddress, builder => builder.ToJson()); // Almacena en columna JSON nativa
}
```

### 3.5. Paginación Eficiente en Servidor
Toda consulta que devuelva colecciones potencialmente grandes debe estar paginada a nivel de base de datos utilizando `.Skip()` y `.Take()`, junto con un conteo previo si es necesario para la UI.

```csharp
public async Task<PagedResult<CustomerSummaryDto>> GetCustomersPagedAsync(
    int pageIndex, 
    int pageSize, 
    CancellationToken ct = default)
{
    var query = db.Customers.AsNoTracking().Where(c => c.IsActive);
    
    var totalCount = await query.CountAsync(ct);
    var items = await query
        .OrderBy(c => c.Name)
        .Skip((pageIndex - 1) * pageSize)
        .Take(pageSize)
        .Select(c => new CustomerSummaryDto(c.Id, c.Name, c.Email, c.Orders.Count))
        .ToListAsync(ct);

    return new PagedResult<CustomerSummaryDto>(items, totalCount, pageIndex, pageSize);
}
```

### 3.6. Rendimiento Extremo: DbContext Pooling y Consultas Compiladas
En proyectos de alta concurrencia:
- Asegúrate de registrar el contexto con `AddDbContextPool<ApplicationDbContext>(...)` en `Program.cs`.
- Si existe una consulta de ruta caliente (hot-path query) ejecutada miles de veces por minuto, utiliza `EF.CompileAsyncQuery`.

---

## 🌐 4. ASP.NET Core Razor Pages — Estándares y Arquitectura de Presentación

Las **Razor Pages** están orientadas a características y páginas (Page-Centric Model). El código dentro de la carpeta `Pages/` debe ser cohesivo y desacoplado de las dependencias de infraestructura profunda.

### 4.1. Estructura y Inyección en `PageModel`
- Aplica **Constructores Primarios** en la declaración del `PageModel`.
- Mantén los manejadores (`OnGetAsync`, `OnPostAsync`) delgados. La lógica de negocio pesada debe delegarse a un servicio o mediador.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.Products;

// ✅ Constructor Primario inyectando el servicio y el logger directamente
public class CreateModel(IProductService productService, ILogger<CreateModel> logger) : PageModel
{
    [BindProperty]
    public required CreateProductInput Input { get; set; }

    public void OnGet()
    {
        // Inicialización simple de la página si es necesario
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        logger.LogInformation("Intentando crear producto: {ProductName}", Input.Name);
        
        var result = await productService.CreateProductAsync(
            new CreateProductCommand(Input.Name, Input.Price, Input.CategoryId), ct);

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return Page();
        }

        TempData["SuccessMessage"] = "Producto creado exitosamente.";
        return RedirectToPage("./Index");
    }
}

// Record de entrada con validación limpia de DataAnnotations para Model Binding
public record CreateProductInput
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(100, MinimumLength = 3)]
    public required string Name { get; init; }

    [Range(0.01, 10000.00, ErrorMessage = "El precio debe ser un valor positivo.")]
    public decimal Price { get; init; }

    [Required]
    public Guid CategoryId { get; init; }
}
```

### 4.2. El Patrón PRG (Post-Redirect-Get)
- **Nunca** devuelvas `Page()` tras una mutación exitosa (`POST`, `PUT`, `DELETE`). Esto provoca el reenvío accidental del formulario en el navegador si el usuario actualiza la página.
- Retorna siempre una redirección (`RedirectToPage()`, `RedirectToPage("./Index")`). Utiliza `TempData` para transportar mensajes efímeros de éxito o notificación entre peticiones.

### 4.3. Tag Helpers sobre HTML Helpers y Lógica Declarativa en Vistas
- En las vistas `.cshtml`, utiliza **Tag Helpers modernos** (`asp-for`, `asp-page`, `asp-items`, `asp-validation-for`) en lugar de los antiguos HTML Helpers (`@Html.TextBoxFor(...)`). Los Tag Helpers proporcionan un HTML visualmente limpio y coherente con el desarrollo frontend moderno.
- Mantén las vistas `.cshtml` libres de código C# complejo (sentencias `if/else` anidadas profundas, consultas LINQ, llamadas a bases de datos). Todo dato computado debe venir preparado y formateado desde el `PageModel`.

```html
<!-- ✅ CORRECTO: Vistas limpias utilizando Tag Helpers ASP.NET Core -->
<form method="post" class="space-y-4">
    <div validation-summary="ModelOnly" class="text-red-600 font-semibold"></div>

    <div>
        <label asp-for="Input.Name" class="block text-sm font-medium text-gray-700"></label>
        <input asp-for="Input.Name" class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500" />
        <span asp-validation-for="Input.Name" class="text-xs text-red-500"></span>
    </div>

    <div>
        <label asp-for="Input.CategoryId" class="block text-sm font-medium text-gray-700">Categoría</label>
        <select asp-for="Input.CategoryId" asp-items="Model.CategoryOptions" class="mt-1 block w-full rounded-md border-gray-300 shadow-sm"></select>
        <span asp-validation-for="Input.CategoryId" class="text-xs text-red-500"></span>
    </div>

    <button type="submit" class="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700">
        Guardar Producto
    </button>
</form>
```

### 4.4. Componentes de Vista (View Components) para Reutilización UI
Cuando necesites fragmentos visuales complejos y reutilizables (ej: carrito de compras en el encabezado, panel de notificaciones en tiempo real, menú de navegación dinámico), **no utilices Partials (`_Partial.cshtml`) con lógica integrada ni pases datos dinámicos mediante `ViewData`**.
- Crea un **ViewComponent** moderno y fuertemente tipado que tenga su propia canalización de acceso a datos asíncrono y vista independiente.

---

## 🏗️ 5. Arquitectura General y Gestión de Errores

### 5.1. Organización por Carpetas de Características (Feature Folders)
Para proyectos de escala media y grande, organiza las páginas por características de dominio en lugar de una estructura monolítica plana.
```text
Pages/
 ├── Shared/
 │    ├── _Layout.cshtml
 │    └── _ValidationScriptsPartial.cshtml
 ├── Customers/
 │    ├── Index.cshtml
 │    ├── Index.cshtml.cs
 │    ├── Create.cshtml
 │    ├── Create.cshtml.cs
 │    ├── Details.cshtml
 │    └── Details.cshtml.cs
 └── Orders/
      ├── Checkout.cshtml
      └── Checkout.cshtml.cs
```

### 5.2. Manejo Global de Excepciones (.NET 8/10 `IExceptionHandler`)
No envuelvas los bloques de los manejadores de Razor Pages en gigantescos try/catch para errores de sistema o caídas de infraestructura. Utiliza el estándar de **Manejo Global de Excepciones** implementando `IExceptionHandler` e inyectándolo en el middleware de `Program.cs`.
- Las excepciones de negocio controladas pueden manejarse mediante el patrón **Result<T>** (o devolviendo objetos con estado de éxito/fallo al `PageModel` para representarlos amigablemente en la interfaz).

### 5.3. Logs Estructurados con Generadores de Código Fuente (`[LoggerMessage]`)
Para operaciones críticas o de alta frecuencia en servicios, utiliza el generador de código fuente `[LoggerMessage]` para evitar la sobrecarga del formateo de strings en tiempo de ejecución.

```csharp
public static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1001, 
        Level = LogLevel.Information, 
        Message = "El usuario {UserId} ha completado la orden {OrderId} por un total de {Amount:C}")]
    public static partial void LogOrderCompleted(this ILogger logger, Guid userId, Guid orderId, decimal amount);
}
```

---

## 📋 6. Checklist de Revisión para el Agente IA

Antes de emitir o modificar cualquier archivo de código, el agente IA DEBE verificar mentalmente esta lista:
- [ ] **¿La sintaxis de la clase/servicio aprovecha C# 14?** (Constructores primarios, keyword `field`, expressions collection, `record` donde sea posible).
- [ ] **¿Los parámetros asíncronos son correctos?** (Todo método `async` pasa el `CancellationToken ct` hasta el ORM o cliente HTTP).
- [ ] **¿Las consultas EF Core de lectura están optimizadas?** (Llevan `.AsNoTracking()`, proyectan con `.Select()` al DTO y no descargan tablas completas en memoria).
- [ ] **¿Las mutaciones masivas de BD son eficientes?** (Se evaluó el uso de `ExecuteUpdateAsync` o `ExecuteDeleteAsync`).
- [ ] **¿El PageModel es conciso y seguro?** (Usa PRG `RedirectToPage()` tras POST, valida `ModelState`, no expone propiedades internas del ORM directamente al model binding).
- [ ] **¿El HTML es limpio y moderno?** (Usa Tag Helpers `asp-*` en lugar de HTML Helpers de estilo ASP.NET MVC 5).
