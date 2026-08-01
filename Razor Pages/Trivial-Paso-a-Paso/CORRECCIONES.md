# Correcciones aplicadas

Esta carpeta parte de la versión publicada en GitHub en el commit
`7d1122744929e8eab7dc06f07e454ea6749ca40a`, del 1 de agosto de 2026.

## Error de compilación de las versiones 5, 6 y 7

En cada una de estas versiones existían los archivos `CategoriaDto.cs` y
`CategoriasDto.cs`. Ambos declaraban el mismo tipo `CategoriaDto`, lo que
provocaba los errores `CS0101` y `CS8863`.

Se conserva únicamente `DTOs/CategoriaDto.cs`, que es el nombre singular usado
por el tipo y por la documentación.

> Si se copian estas correcciones sobre una carpeta anterior, el archivo
> `DTOs/CategoriasDto.cs` no desaparece automáticamente. Es preferible sustituir
> la carpeta completa o eliminar expresamente ese archivo en las versiones 5,
> 6 y 7 antes de ejecutar `dotnet run`.

## Dependencias de SQLite

Los siete proyectos utilizan ahora:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.10" />
<PackageReference Include="SQLitePCLRaw.bundle_e_sqlite3" Version="2.1.12" />
```

La referencia directa a `SQLitePCLRaw.bundle_e_sqlite3` evita que NuGet
restaure la versión vulnerable `2.1.11` y elimina la advertencia `NU1903`
indicada en el enunciado.

Después de sustituir los archivos, conviene limpiar y restaurar cada proyecto:

```powershell
dotnet clean
dotnet nuget locals all --clear
dotnet restore
dotnet run
```

## Compatibilidad con Linux y MonsterASP.NET

En las siete etapas y en el proyecto de pruebas, `_layout.cshtml` se ha
renombrado como `_Layout.cshtml`. Windows no suele distinguir las mayúsculas en
el nombre, pero Linux sí; `_ViewStart.cshtml` solicita expresamente `_Layout`.

Si se aplica el cambio desde Git en Windows y no detecta el cambio de
mayúsculas, puede hacerse en dos pasos:

```powershell
git mv Pages/Shared/_layout.cshtml Pages/Shared/_Layout.tmp
git mv Pages/Shared/_Layout.tmp Pages/Shared/_Layout.cshtml
```

Debe repetirse dentro de `TrivialApi` para cada etapa que se quiera actualizar.

## API, CORS y clientes

- Las versiones 5, 6 y 7 ya configuran la política CORS `PermitirTodos`.
- Los clientes integrados de las versiones 6 y 7 utilizan `/api`, por lo que
  no dependen de un puerto local ni requieren cambios al publicar.
- El cliente del proyecto de pruebas también utiliza `/api`.
- Los clientes integrados desactivan los botones de respuesta después del
  primer clic para evitar dos respuestas simultáneas.
- Los README explican ahora que el cliente JavaScript independiente puede
  conectarse a las versiones 5, 6 o 7, aunque se recomienda la 7 por ser la más
  completa.

## Proyecto de pruebas

- El DTO y el contrato JSON se han igualado con los de las versiones 5, 6 y 7.
- La prueba del filtro por categoría consulta `pregunta.Categoria.Id`.
- La copia de `Data/trivial.db` contiene las mismas 10 categorías y 1.000
  preguntas que el itinerario principal. Las pruebas continúan utilizando una
  base SQLite independiente y en memoria.
- El README indica la carpeta real desde la que se ejecuta `dotnet test`.

## Limpieza del repositorio y documentación

- Se ha eliminado `07-Version-Definitiva/TrivialApi/trivial.zip`, que era una
  copia generada y desactualizada del código fuente.
- Se ha añadido `*.zip` a `.gitignore`.
- Se ha retirado del README el enlace a `Clientes/HTML y JS`, carpeta que ya no
  existe en GitHub.
- Se han corregido las instrucciones sobre CORS, `/api`, dependencias y la
  estructura del proyecto.

## Comprobación recomendada

Desde cada carpeta `TrivialApi`:

```powershell
dotnet restore
dotnet build
dotnet run
```

Y desde `Pruebas`:

```powershell
dotnet test
```

