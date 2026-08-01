# Versión 7: aplicación definitiva

Esta es la versión final. Conserva todo el código funcional anterior y añade
las mejoras de interfaz y experiencia de usuario.

No se introduce otra capa de acceso a datos ni se modifica el diseño de la
API. Los cambios se concentran en el layout, las vistas y dos archivos
JavaScript.

## Funcionalidad completa

- SQLite con 1.000 preguntas.
- Entity Framework Core.
- CRUD de categorías.
- CRUD de preguntas.
- Validaciones.
- Búsqueda automática.
- Búsqueda sin mayúsculas ni tildes.
- Filtro por categoría.
- Paginación.
- API REST.
- DTO.
- CORS.
- Cliente del juego.
- Resultados con SweetAlert.
- Confirmación de borrado con SweetAlert.
- Selector de temas.
- Bootstrap claro y oscuro.
- Bootswatch.
- Bootstrap Icons.
- Adaptación móvil.
- Enlaces directos a JSON.

## Ejecutar

```bash
cd 07-Version-Definitiva/TrivialApi
dotnet restore
dotnet run
```

Administración:

```text
/
```

Cliente:

```text
/cliente/index.html
```

## Archivos añadidos

```text
wwwroot/js
├── temas.js
└── sweetalert.js
```

## Archivos ampliados

```text
Pages/Shared/_Layout.cshtml
Pages/Categorias/Index.cshtml
Pages/Preguntas/Index.cshtml
wwwroot/cliente/index.html
```

`wwwroot/cliente/trivial.js` no necesita cambios. La lógica del juego de la
versión 6 permanece igual.

## SweetAlert y TempData

Los PageModels siguen escribiendo:

```csharp
TempData["Mensaje"] = "...";
```

El layout lee el mensaje y lo copia a atributos:

```html
data-mensaje="..."
data-icono="success"
data-titulo="Operación completada"
```

`sweetalert.js` lee:

```javascript
document.body.dataset.mensaje
```

La lógica del servidor no depende de SweetAlert. Solo cambia la forma de
presentar el mensaje en el navegador.

## Confirmación reutilizable

Los formularios de borrado comparten:

```html
class="formulario-eliminar"
```

También incluyen:

```html
data-elemento="la pregunta"
data-nombre="..."
```

Las categorías añaden:

```html
data-aviso="También se eliminarán..."
```

Un único script recorre todos los formularios y registra el mismo comportamiento.

## evento.preventDefault

Al comenzar un envío:

```javascript
evento.preventDefault();
```

detiene el POST mientras SweetAlert espera la respuesta.

Si se confirma:

```javascript
formulario.submit();
```

envía el formulario al mismo handler que existía en las versiones anteriores.

## Dataset

Un atributo:

```html
data-nombre="Historia"
```

se consulta como:

```javascript
formulario.dataset.nombre
```

La conversión elimina `data-` y transforma nombres con guion a camelCase.

## Selector de temas

El selector diferencia:

- Bootstrap claro.
- Bootstrap oscuro.
- Temas Bootswatch.

Los valores Bootswatch comienzan por:

```text
bootswatch-
```

Esto permite reconocerlos sin mantener otra propiedad adicional.

## Sustituir la hoja de estilos

El layout y el cliente tienen:

```html
<link id="temaCss" ...>
```

`temas.js` modifica su propiedad `href`.

Para Bootswatch construye:

```javascript
`https://cdn.jsdelivr.net/npm/bootswatch@5.3.8/dist/${nombre}/bootstrap.min.css`
```

Para Bootstrap claro u oscuro utiliza la hoja normal.

## Tema claro y oscuro

La hoja de Bootstrap es la misma en los dos modos. La diferencia se indica:

```javascript
document.documentElement.dataset.bsTheme =
    "dark";
```

Esto produce:

```html
<html data-bs-theme="dark">
```

Bootstrap adapta fondos, bordes, controles y texto.

## Lista de temas oscuros

Bootswatch no comunica automáticamente su modo mediante el nombre de la hoja.
Por eso se mantiene una lista explícita.

La lista existe una sola vez en `temas.js`.

## localStorage

La elección se guarda con:

```javascript
localStorage.setItem(
    "temaTrivial",
    tema
);
```

Y se recupera con:

```javascript
localStorage.getItem("temaTrivial")
    ?? "bootstrap-light";
```

El operador `??` utiliza el tema claro si todavía no existe un valor.

## Código compartido con el cliente

El cliente carga:

```html
<script src="../js/temas.js"></script>
```

No contiene selector, pero el script lo comprueba:

```javascript
if (selectorTema)
{
    ...
}
```

Así:

- Razor muestra y escucha el selector.
- El cliente solo aplica la elección guardada.
- Ambos comparten construcción de URL y lista de temas oscuros.

## Iconos

Bootstrap Icons se carga desde CDN.

Los botones compactos mantienen accesibilidad mediante:

```html
title="Editar"
aria-label="Editar"
```

Cuando se oculta texto visual:

```html
<span class="visually-hidden">
    Nueva pregunta
</span>
```

el lector de pantalla todavía dispone de una descripción.

## Enlaces JSON

En el listado de preguntas, el enunciado enlaza a:

```text
/api/preguntas/{id}
```

En el listado de categorías, el nombre enlaza a:

```text
/api/preguntas?categoriaId={id}&cantidad=100
```

Se abren en otra pestaña mediante `target="_blank"` y se añade
`rel="noopener"`.

## Diseño responsive

Se utilizan exclusivamente clases Bootstrap:

- `navbar-expand-lg`.
- `table-responsive`.
- `flex-wrap`.
- `col-md-6`.
- `col-lg-*`.
- `p-4 p-md-5`.
- `text-nowrap`.
- `d-grid`.

No existe un archivo CSS personalizado.

## Recorrido completo de administración

```text
Formulario Razor
      ↓ POST
PageModel
      ↓
Entity Framework
      ↓
SQLite
      ↓ Redirect
TempData
      ↓
SweetAlert
```

## Recorrido completo del juego

```text
Cliente HTML
      ↓
trivial.js
      ↓ fetch
Controlador API
      ↓
DTO
      ↓
JSON
      ↓
Botones y SweetAlert
```

## Diferencias con la versión 6

| Versión 6 | Versión 7 |
|---|---|
| Bootstrap fijo | Selector Bootstrap/Bootswatch |
| Tema no persistente | `localStorage` |
| Alerta Bootstrap en CRUD | SweetAlert |
| `confirm()` | Confirmación reutilizable |
| Botones con texto | Iconos accesibles |
| Texto normal | Enlaces al JSON |
| Cliente claro | Tema compartido |

## Pruebas manuales

### Administración

1. Crear categoría.
2. Editar categoría.
3. Cancelar su borrado.
4. Confirmar su borrado.
5. Comprobar el aviso de cascada.
6. Crear pregunta.
7. Editar pregunta.
8. Cancelar su borrado.
9. Confirmar su borrado.
10. Probar validaciones.

### Búsqueda

1. Buscar con mayúsculas.
2. Buscar con minúsculas.
3. Buscar con tilde.
4. Buscar sin tilde.
5. Filtrar por categoría.
6. Combinar filtros.
7. Cambiar de página.
8. Comprobar el cursor.

### API

1. Abrir todos los enlaces de inicio.
2. Abrir el JSON desde una pregunta.
3. Abrir el JSON desde una categoría.
4. Probar un Id inexistente.

### Cliente

1. Jugar con todas las categorías.
2. Jugar con una categoría.
3. Acertar.
4. Fallar.
5. Terminar.
6. Volver a jugar.

### Temas

1. Seleccionar Bootstrap oscuro.
2. Probar varios Bootswatch claros.
3. Probar varios Bootswatch oscuros.
4. Recargar Razor Pages.
5. Abrir el cliente.
6. Cerrar y volver a abrir el navegador.

## Preguntas para el alumnado

1. ¿Por qué TempData sigue siendo útil con SweetAlert?
2. ¿Qué diferencia hay entre `dataset` y `localStorage`?
3. ¿Por qué se llama a `preventDefault`?
4. ¿Cómo se reutiliza la confirmación?
5. ¿Qué cambia realmente al elegir Bootswatch?
6. ¿Para qué sirve `data-bs-theme`?
7. ¿Por qué el cliente no necesita otro selector?
8. ¿Qué aporta `aria-label`?
9. ¿Por qué los enlaces JSON utilizan `noopener`?
10. ¿Qué partes funcionan sin JavaScript?

## Ejercicios sugeridos

1. Añadir un botón para restaurar el tema claro.
2. Guardar la última categoría jugada.
3. Añadir un selector de número de preguntas.
4. Mostrar un resumen de errores al terminar.
5. Añadir un endpoint de estadísticas.
6. Incorporar ordenación al listado.
7. Limitar CORS a un origen concreto.

## Conclusión

La aplicación final combina varias interfaces sin duplicar el acceso a datos:

- Razor Pages administra.
- Los controladores publican.
- El cliente consume.
- Entity Framework centraliza SQLite.
- Bootstrap presenta.
- JavaScript añade comportamientos puntuales.

La evolución desde la versión 1 permite estudiar cada responsabilidad antes de
integrarla con las demás.
