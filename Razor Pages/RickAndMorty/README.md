# Rick and Morty — Razor Pages

Aplicación educativa desarrollada con **ASP.NET Core Razor Pages y .NET 10**.
Consume [The Rick and Morty API](https://rickandmortyapi.com/), permite crear
cuentas locales y guarda los personajes favoritos de cada usuario en SQLite.

La API es pública y no requiere token, clave ni registro.

## Funcionalidades

- Portada con cifras generales y personajes destacados.
- Catálogo paginado de personajes.
- Filtros por nombre, estado, especie y género.
- Ficha completa con origen, última localización y apariciones.
- Guía de episodios con búsqueda por título o código.
- Detalle de cada episodio y todos sus personajes.
- Catálogo de localizaciones con nombre, tipo y dimensión.
- Detalle de localización y selección de residentes.
- Navegación entre recursos relacionados.
- Registro e inicio de sesión con ASP.NET Core Identity.
- Registro inmediato, sin confirmación obligatoria por correo.
- Colección privada de personajes favoritos por usuario.
- SQLite creada automáticamente en el primer arranque.
- Caché en memoria para reducir llamadas externas.
- Bootstrap, Bootswatch, Bootstrap Icons y SweetAlert2 desde CDN.
- Selector de temas guardado mediante `localStorage`.
- Mensajes y confirmaciones con SweetAlert2.
- Interfaz adaptable a ordenador, tableta y móvil.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Conexión a internet para la API y los recursos CDN.

No hay que obtener ni configurar ninguna clave de acceso.

## Puesta en marcha

Abre un terminal en la carpeta que contiene `RickAndMorty.csproj`:

```powershell
cd "ruta\hasta\RickAndMorty"
dotnet restore
dotnet run
```

Abre la dirección que aparece en la consola, normalmente:

```text
https://localhost:7174
```

o:

```text
http://localhost:5193
```

## Registro e inicio de sesión

Identity se configura en `Program.cs` con:

- correo electrónico único;
- contraseña de al menos 8 caracteres;
- una mayúscula, una minúscula y un número;
- bloqueo de cinco minutos después de cinco intentos fallidos;
- confirmación de correo desactivada.

El usuario inicia sesión automáticamente al registrarse. Las contraseñas no
se guardan en texto plano: Identity aplica hash, sal y sus mecanismos de
seguridad internos.

## Base de datos

La aplicación crea `Data/rickandmorty.db` en el primer arranque mediante
`EnsureCreatedAsync()`. En ella se guardan:

- las tablas de usuarios, roles, cookies y tokens de Identity;
- la tabla `PersonajesFavoritos`.

Existe un índice único para `UsuarioId + PersonajeId`. Un usuario no puede
guardar dos veces el mismo personaje, pero usuarios diferentes sí pueden
guardar el mismo.

Para reiniciar todos los usuarios y favoritos durante una práctica:

1. Detén la aplicación.
2. Elimina `Data/rickandmorty.db`.
3. Ejecuta de nuevo `dotnet run`.

No lo hagas si necesitas conservar los datos.

## Estructura

```text
RickAndMorty/
├── Configuracion/        Opciones de caché
├── Data/                 DbContext y base de datos en ejecución
├── DTOs/                 Respuestas JSON de la API
├── Modelos/              Usuario, favorito y paginación
├── Pages/
│   ├── Cuenta/           Registro, login y logout
│   ├── Episodios/        Listado y detalle
│   ├── Favoritos/        Colección privada y acciones POST
│   ├── Localizaciones/   Listado, filtros y residentes
│   ├── Personajes/       Catálogo, filtros y ficha completa
│   └── Shared/           Layout, tarjetas y paginación
├── Servicios/            API, caché, favoritos y traducciones
└── wwwroot/              JavaScript e imagen alternativa
```

## Endpoints externos utilizados

| Función | Endpoint |
|---|---|
| Personajes | `GET /api/character` |
| Personaje individual | `GET /api/character/{id}` |
| Varios personajes | `GET /api/character/{id1,id2,...}` |
| Episodios | `GET /api/episode` |
| Episodio individual | `GET /api/episode/{id}` |
| Varios episodios | `GET /api/episode/{id1,id2,...}` |
| Localizaciones | `GET /api/location` |
| Localización individual | `GET /api/location/{id}` |

Todos los listados aceptan los filtros documentados por la API. Cada página
contiene hasta 20 resultados.

## Cómo funciona el servicio de la API

`RickAndMortyServicio` es el único componente que conoce la dirección de la
API. Sus responsabilidades son:

1. Construir las URL y sus filtros.
2. Realizar peticiones asíncronas con `HttpClient`.
3. Deserializar JSON en DTO.
4. Guardar resultados en `IMemoryCache`.
5. Convertir los errores técnicos en mensajes comprensibles.
6. Obtener varios recursos relacionados en una única petición.

Una búsqueda sin resultados recibe un código HTTP 404 de esta API. El
servicio lo transforma en una página vacía para que la interfaz pueda mostrar
«No hay resultados» en lugar de tratarlo como un error.

## Relaciones entre recursos

La API no devuelve objetos completos dentro de las relaciones, sino URL:

- un personaje contiene las URL de sus episodios;
- un episodio contiene las URL de sus personajes;
- una localización contiene las URL de sus residentes;
- un personaje contiene la URL de su origen y localización actual.

`TextoRickAndMorty.ExtraerId()` obtiene el identificador final de cada URL y el
servicio utiliza los endpoints de múltiples identificadores para reducir el
número de peticiones.

Las localizaciones con más de 40 residentes muestran los primeros 40 para
evitar direcciones excesivamente largas y respuestas innecesariamente
pesadas. La interfaz lo indica expresamente.

## Caché

La duración predeterminada es de 30 minutos:

```json
{
  "RickAndMortyApi": {
    "MinutosCache": 30
  }
}
```

Se puede cambiar en `appsettings.json`. El servicio limita el valor efectivo
entre 5 y 240 minutos.

## Recursos desde CDN

El proyecto no instala dependencias front-end:

- Bootstrap 5.3.8
- Bootswatch 5.3.8
- Bootstrap Icons 1.13.1
- SweetAlert2 11.26.25

El selector permite cambiar entre Bootstrap claro, Bootstrap oscuro y varios
temas Bootswatch. La elección se conserva en el navegador.

## Seguridad

- Los cambios en favoritos utilizan formularios POST.
- Razor Pages añade automáticamente el token antifalsificación.
- Las páginas privadas llevan el atributo `[Authorize]`.
- Cada consulta SQLite filtra por el identificador del usuario.
- Las URL de retorno se validan con `Url.IsLocalUrl`.
- Los errores externos no muestran trazas internas en la interfaz.

## Posibles ampliaciones para clase

- Añadir notas personales a los favoritos.
- Permitir ordenar personajes por nombre o número de episodios.
- Crear una lista de episodios vistos por usuario.
- Guardar una valoración personal de cada episodio.
- Sustituir `EnsureCreated` por migraciones de Entity Framework.
- Añadir pruebas unitarias para servicios y PageModel.
- Crear una API propia de favoritos.
- Comparar el consumo REST con el endpoint GraphQL disponible.

## Créditos y licencia

Datos e imágenes proporcionados por
[The Rick and Morty API](https://rickandmortyapi.com/).

Rick and Morty y sus personajes pertenecen a sus respectivos titulares. Este
repositorio es un proyecto educativo no oficial.

El código del proyecto se distribuye con licencia MIT.
