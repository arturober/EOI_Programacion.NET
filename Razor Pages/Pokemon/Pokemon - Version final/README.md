# Pokédex con Razor Pages y PokeAPI

Proyecto educativo realizado con **ASP.NET Core Razor Pages**, **Bootstrap
5.3.8**, **Bootstrap Icons 1.13.1** y **Bootswatch 5.3.8**. Todas las
librerías visuales se cargan desde CDN, por lo que no es necesario instalar
paquetes de interfaz.

El código está escrito para estudiantes que están aprendiendo C#. Se ha
priorizado la legibilidad: las páginas contienen poca lógica, las llamadas HTTP
están reunidas en un servicio y todos los bloques importantes incluyen
comentarios normales en español.

## Funcionalidad

- Portada independiente y listado paginado de todos los Pokémon y sus variedades.
- Búsqueda parcial por nombre mientras se escribe.
- Debounce de 500 ms escrito directamente en el elemento `input`.
- Detalle accesible por nombre o identificador.
- Nombre, descripción y categoría en español cuando PokeAPI los proporciona.
- Datos físicos y biológicos, tipos, habilidades y estadísticas.
- Formas, variedades, cadena evolutiva y versiones de los juegos.
- Objetos, lugares de encuentro y lista completa de movimientos.
- Carrusel automático con todas las imágenes encontradas en `sprites`,
  incluidas las de versiones antiguas.
- Reproductores para el sonido actual y el sonido clásico.
- Respuesta JSON completa para estudiar cualquier campo no representado
  visualmente.
- Diseño responsive y accesible basado en Bootstrap.
- Bootstrap claro, Bootstrap oscuro y todos los temas actuales de Bootswatch.
- Temas agrupados en el desplegable mediante `optgroup`.
- El tema elegido queda guardado en `localStorage` para las próximas visitas.
- Botones del carrusel con contraste suficiente en todos los temas.
- Ningún archivo CSS propio y ningún archivo JavaScript propio.
- Caché de treinta minutos para la lista utilizada por el buscador.
- Tratamiento de Pokémon inexistentes, errores de conexión y tiempo de espera.

## Endpoints utilizados

```text
GET https://pokeapi.co/api/v2/pokemon?limit=100000&offset=0
GET https://pokeapi.co/api/v2/pokemon/{nombre-o-id}
GET https://pokeapi.co/api/v2/pokemon-species/{id}
GET https://pokeapi.co/api/v2/pokemon/{id}/encounters
GET https://pokeapi.co/api/v2/evolution-chain/{id}
```

## Requisitos

- SDK de .NET 10.
- Conexión a Internet para consultar PokeAPI y cargar los CDN.

Puedes comprobar la versión instalada con:

```bash
dotnet --version
```

## Ejecutar el proyecto

Abre una terminal dentro de la carpeta `Pokemon` y ejecuta:

```bash
dotnet restore
dotnet run
```

La terminal mostrará una dirección local parecida a:

```text
https://localhost:7000
```

Ábrela en el navegador. Si el certificado local todavía no es de confianza,
puedes prepararlo con:

```bash
dotnet dev-certs https --trust
```

## Publicación en MonsterASP.NET

> **Método recomendado:** utiliza WebFTP.

La versión final no necesita una clave de PokeAPI, cuentas de usuario ni base de
datos. Desde la terminal integrada de VS Code:

```bash
dotnet restore
dotnet publish -c Release -o publicacion
```

En PowerShell:

```powershell
Compress-Archive -Path .\publicacion\* -DestinationPath .\publicacion.zip -Force
```

Sube y extrae `publicacion.zip` dentro de `/wwwroot` mediante **Files**. En
la raíz deben quedar `Pokemon.dll`, `web.config`, `appsettings.json` y
`wwwroot`. No subas todo el itinerario ni el código fuente.

El servidor necesita acceso a PokeAPI y GitHub Raw. El navegador necesita
acceso a los CDN de Bootstrap, Bootswatch y Bootstrap Icons. La caché de treinta
minutos está en memoria y se vacía al reiniciar la aplicación.

### Comprobar el despliegue

1. Abre la portada y el listado.
2. Busca por una parte del nombre.
3. Abre un detalle y comprueba evolución, movimientos y encuentros.
4. Prueba el carrusel y los sonidos.
5. Cambia entre temas claros y oscuros.
6. Reinicia la aplicación y comprueba que vuelve a consultar la lista.

Un primer acceso lento puede deberse a que la caché todavía está vacía. Si
PokeAPI devuelve errores, revisa los logs y evita recargar masivamente el sitio.
Consulta la
[guía de publicación mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

### Método recomendado: WebFTP

Para estos proyectos se recomienda **WebFTP**, el cliente FTP que
MonsterASP.NET ofrece en el navegador. No requiere instalar programas ni crear
o extraer un ZIP.

1. Desde la terminal integrada de VS Code, genera o actualiza la carpeta
   `publicacion` con el comando `dotnet publish` indicado anteriormente.
2. En el panel de MonsterASP.NET abre el sitio y entra en
   **Files → WebFTP**. También se puede acceder a
   [WebFTP](https://webftp.monsterasp.net/) desde el enlace que muestra el
   panel.
3. Dentro de WebFTP, abre `/wwwroot`.
4. Sube **el contenido** de `publicacion`, no la carpeta como un nivel
   adicional. `web.config`, el ensamblado principal, `appsettings.json` y
   las demás carpetas publicadas deben quedar directamente en `/wwwroot`.
5. En las actualizaciones, conserva las bases de datos y los demás archivos
   persistentes indicados en este README.
6. Si algún archivo está bloqueado, reinicia o detén temporalmente el sitio.
   También puedes subir `app_offline.htm` a `/wwwroot`, completar la
   transferencia, eliminarlo y volver a iniciar la aplicación.
7. Abre la dirección HTTPS del sitio y realiza las comprobaciones específicas
   indicadas en este README.

Como segunda opción, para transferencias grandes o frecuentes, puede utilizarse
FileZilla u otro cliente FTP/SFTP con las credenciales disponibles en
**Deploy (FTP/WebDeploy/Git)**. La
[guía oficial de FTP/SFTP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-via-ftpsftp)
explica esta alternativa.

### Alternativa: despliegue mediante ZIP

El procedimiento mediante ZIP descrito anteriormente se mantiene disponible.
Desde **Files**, sube `publicacion.zip`, pulsa **Unzip** y elige `/wwwroot`
como destino. Al actualizar una aplicación existente, marca
**Overwrite files in target path** y
**Restart application pool before unzip** para sustituir los archivos en uso.
El contenido publicado debe quedar directamente en `/wwwroot`, sin una
carpeta `publicacion` intermedia, y deben respetarse las indicaciones de este
README sobre bases de datos y otros archivos persistentes.

Consulta la
[guía oficial de despliegue mediante ZIP](https://help.monsterasp.net/books/deploy/page/how-to-deploy-website-content-from-zip-file).

## Estructura

```text
Pokemon/
├── Models/
│   ├── PokemonModelos.cs
│   └── RespuestasPokeApi.cs
├── Pages/
│   ├── Shared/_Layout.cshtml
│   ├── Index.cshtml
│   ├── Index.cshtml.cs
│   ├── Listado.cshtml
│   ├── Listado.cshtml.cs
│   ├── Detalle.cshtml
│   ├── Detalle.cshtml.cs
│   ├── Error.cshtml
│   └── Error.cshtml.cs
├── Services/
│   └── PokeApiService.cs
├── Program.cs
└── Pokemon.csproj
```

## Flujo de una petición

1. El navegador abre una página Razor.
2. El `PageModel` solicita los datos a `PokeApiService`.
3. El servicio llama de forma asíncrona a PokeAPI.
4. `System.Text.Json` convierte el JSON en objetos de C#.
5. El servicio prepara modelos sencillos para la vista.
6. Razor genera el HTML y Bootstrap se ocupa de la presentación.

SweetAlert no se ha añadido porque la aplicación no realiza acciones
destructivas ni necesita confirmaciones modales. Los avisos de conexión se
muestran mediante componentes `alert` de Bootstrap.

No existe `site.css`: toda la presentación utiliza clases de Bootstrap. Tampoco
existe `site.js`: el debounce está en el atributo `oninput` del buscador y el
pequeño código necesario para cambiar de tema está comentado dentro de
`_Layout.cshtml`.
