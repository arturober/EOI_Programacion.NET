using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace TrivialApi.PlaywrightTests;

[Collection(PlaywrightTestCollection.Nombre)]
public sealed class CategoriasTests(
    PlaywrightFixture aplicacion,
    ITestOutputHelper salida)
    : PlaywrightTestBase(aplicacion, salida)
{
    [Fact(DisplayName = "Playwright: se puede crear una categoría")]
    public async Task CrearCategoria_MuestraLaNuevaCategoria()
    {
        string nombre = NombreUnico("Categoría PW");
        Informe.Inicio($"Crear la categoría {nombre}");

        await CrearCategoriaAsync(nombre);
        await AbrirAsync("/Categorias");

        Informe.Comprobacion("La nueva categoría aparece en el listado");
        await Expect(Page.GetByRole(AriaRole.Row).Filter(new() { HasText = nombre })).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: una categoría repetida muestra validación")]
    public async Task CrearCategoria_Repetida_MuestraError()
    {
        string nombre = NombreUnico("Repetida PW");
        Informe.Inicio("Intentar crear dos categorías con el mismo nombre");
        await CrearCategoriaAsync(nombre);

        await AbrirAsync("/Categorias/Crear");
        await Page.Locator("input[name$='Nombre']").FillAsync(nombre);
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Crear|Guardar", RegexOptions.IgnoreCase) }).ClickAsync();

        Informe.Comprobacion("La página muestra el error de nombre duplicado");
        await Expect(Page.GetByText(new Regex("Ya existe.*categoría", RegexOptions.IgnoreCase))).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: se puede editar una categoría")]
    public async Task EditarCategoria_CambiaElNombre()
    {
        string nombre = NombreUnico("Editar PW");
        string nuevoNombre = NombreUnico("Editada PW");
        Informe.Inicio($"Editar {nombre} y convertirla en {nuevoNombre}");
        await CrearCategoriaAsync(nombre);

        await AbrirAsync("/Categorias");
        ILocator fila = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = nombre });
        await fila.Locator("a[aria-label='Editar']").ClickAsync();
        await Page.Locator("input[name$='Nombre']").FillAsync(nuevoNombre);
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Guardar|Actualizar", RegexOptions.IgnoreCase) }).ClickAsync();
        await CerrarSweetAlertSiExisteAsync();

        Informe.Comprobacion("El listado contiene el nombre editado");
        await Expect(Page.GetByRole(AriaRole.Row).Filter(new() { HasText = nuevoNombre })).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: SweetAlert permite cancelar y confirmar el borrado de una categoría")]
    public async Task EliminarCategoria_CancelarYConfirmar()
    {
        string nombre = NombreUnico("Eliminar PW");
        Informe.Inicio($"Cancelar y después confirmar el borrado de {nombre}");
        await CrearCategoriaAsync(nombre);
        await AbrirAsync("/Categorias");

        ILocator fila = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = nombre });
        ILocator eliminar = fila.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Eliminar", RegexOptions.IgnoreCase) });

        Informe.Paso("Abriendo la confirmación y pulsando Cancelar");
        await eliminar.ClickAsync();
        await Expect(Page.Locator(".swal2-popup")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Cancelar" }).ClickAsync();
        await Expect(fila).ToBeVisibleAsync();

        Informe.Paso("Abriendo de nuevo la confirmación y eliminando");
        await eliminar.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Sí, eliminar", RegexOptions.IgnoreCase) }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Row).Filter(new() { HasText = nombre })).ToHaveCountAsync(0);
        Informe.Exito();
    }

    private async Task CrearCategoriaAsync(string nombre)
    {
        await AbrirAsync("/Categorias/Crear");
        await Page.Locator("input[name$='Nombre']").FillAsync(nombre);
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Crear|Guardar", RegexOptions.IgnoreCase) }).ClickAsync();
        await CerrarSweetAlertSiExisteAsync();
    }
}
