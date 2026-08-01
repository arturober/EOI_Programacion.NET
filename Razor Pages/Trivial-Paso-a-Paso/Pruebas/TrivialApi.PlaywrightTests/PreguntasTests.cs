using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Xunit;
using Xunit.Abstractions;
using static Microsoft.Playwright.Assertions;

namespace TrivialApi.PlaywrightTests;

[Collection(PlaywrightTestCollection.Nombre)]
public sealed class PreguntasTests(
    PlaywrightFixture aplicacion,
    ITestOutputHelper salida)
    : PlaywrightTestBase(aplicacion, salida)
{
    [Fact(DisplayName = "Playwright: se puede crear una pregunta completa")]
    public async Task CrearPregunta_MuestraLaNuevaPregunta()
    {
        string enunciado = NombreUnico("Pregunta PW");
        Informe.Inicio($"Crear la pregunta {enunciado}");
        await CrearPreguntaAsync(enunciado);

        await AbrirPreguntaEnListadoAsync(enunciado);
        Informe.Comprobacion("La pregunta aparece en el listado");
        await Expect(Page.GetByRole(AriaRole.Row).Filter(new() { HasText = enunciado })).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: el formulario de pregunta muestra validaciones")]
    public async Task CrearPregunta_Vacia_MuestraValidaciones()
    {
        Informe.Inicio("Enviar el formulario de pregunta sin datos");
        await AbrirAsync("/Preguntas/Crear");
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Crear|Guardar", RegexOptions.IgnoreCase) }).ClickAsync();

        Informe.Comprobacion("La pregunta, las respuestas y la categoría muestran sus errores");
        await Expect(Page.Locator("[data-valmsg-for$='Enunciado']"))
            .ToContainTextAsync("La pregunta es obligatoria.");
        await Expect(Page.Locator("[data-valmsg-for$='Respuesta1']"))
            .ToContainTextAsync("La respuesta 1 es obligatoria.");
        await Expect(Page.Locator("[data-valmsg-for$='CategoriaId']"))
            .ToContainTextAsync(new Regex("Selecciona una categoría", RegexOptions.IgnoreCase));
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: se puede editar una pregunta")]
    public async Task EditarPregunta_CambiaElEnunciado()
    {
        string original = NombreUnico("Original PW");
        string editado = NombreUnico("Editada PW");
        Informe.Inicio("Crear una pregunta y modificar su enunciado");
        await CrearPreguntaAsync(original);
        await AbrirPreguntaEnListadoAsync(original);

        ILocator fila = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = original });
        await fila.Locator("a[aria-label='Editar']").ClickAsync();
        await Page.Locator("[name$='Enunciado']").FillAsync(editado);
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Guardar|Actualizar", RegexOptions.IgnoreCase) }).ClickAsync();
        await CerrarSweetAlertSiExisteAsync();

        await AbrirPreguntaEnListadoAsync(editado);
        Informe.Comprobacion("El listado muestra el enunciado modificado");
        await Expect(Page.GetByRole(AriaRole.Row).Filter(new() { HasText = editado })).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: la búsqueda ignora mayúsculas y tildes")]
    public async Task Busqueda_SinMayusculasNiTildes_EncuentraPregunta()
    {
        Informe.Inicio("Buscar 'quien pinto' sin mayúsculas ni tildes");
        await AbrirAsync("/Preguntas");

        ILocator busqueda = Page.Locator("[name='Busqueda']");
        await busqueda.FillAsync("quien pinto");
        await Page.WaitForTimeoutAsync(800);

        Informe.Comprobacion("Se encuentra la pregunta de Las Meninas");
        await Expect(Page.GetByText("¿Quién pintó Las Meninas?")).ToBeVisibleAsync();
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: el buscador conserva el foco tras filtrar")]
    public async Task Busqueda_Automatica_ConservaElFoco()
    {
        Informe.Inicio("Escribir en el buscador y comprobar el cursor");
        await AbrirAsync("/Preguntas");

        ILocator busqueda = Page.Locator("[name='Busqueda']");
        await busqueda.FillAsync("planeta");
        await Page.WaitForTimeoutAsync(800);

        bool tieneFoco = await busqueda.EvaluateAsync<bool>(
            "elemento => document.activeElement === elemento");
        Informe.Comprobacion("El campo de búsqueda continúa activo");
        Assert.True(tieneFoco);
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: el filtro de categoría limita los resultados")]
    public async Task FiltroCategoria_MuestraSoloLaCategoriaSeleccionada()
    {
        Informe.Inicio("Filtrar el listado por la categoría Ciencia");
        await AbrirAsync("/Preguntas");

        ILocator selector = Page.Locator("[name='CategoriaId']");
        await selector.SelectOptionAsync(new SelectOptionValue { Label = "Ciencia" });

        // Registramos la espera antes de enviar el formulario. Esperar únicamente
        // DOMContentLoaded podría resolverse sobre la página anterior.
        Task esperaNavegacion = Page.WaitForURLAsync(
            new Regex(@"[?&]CategoriaId=\d+"));

        await Page.Locator("#formularioBusqueda").EvaluateAsync(
            "formulario => formulario.requestSubmit()");
        await esperaNavegacion;

        Informe.Comprobacion("Las filas de datos pertenecen a Ciencia");
        ILocator filas = Page.Locator("tbody tr");
        int cantidad = await filas.CountAsync();
        Assert.True(cantidad > 0);
        for (int indice = 0; indice < cantidad; indice++)
        {
            await Expect(filas.Nth(indice)).ToContainTextAsync("Ciencia");
        }
        Informe.Exito();
    }

    [Fact(DisplayName = "Playwright: SweetAlert permite cancelar y confirmar el borrado de una pregunta")]
    public async Task EliminarPregunta_CancelarYConfirmar()
    {
        string enunciado = NombreUnico("Eliminar pregunta PW");
        Informe.Inicio("Cancelar y después confirmar el borrado de una pregunta");
        await CrearPreguntaAsync(enunciado);
        await AbrirPreguntaEnListadoAsync(enunciado);

        ILocator fila = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = enunciado });
        ILocator eliminar = fila.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Eliminar", RegexOptions.IgnoreCase) });

        await eliminar.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Cancelar" }).ClickAsync();
        await Expect(fila).ToBeVisibleAsync();

        await eliminar.ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Sí, eliminar", RegexOptions.IgnoreCase) }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Row).Filter(new() { HasText = enunciado })).ToHaveCountAsync(0);
        Informe.Exito();
    }

    private async Task CrearPreguntaAsync(string enunciado)
    {
        await AbrirAsync("/Preguntas/Crear");
        await Page.Locator("[name$='Enunciado']").FillAsync(enunciado);
        await Page.Locator("[name$='Respuesta1']").FillAsync("Respuesta correcta");
        await Page.Locator("[name$='Respuesta2']").FillAsync("Respuesta dos");
        await Page.Locator("[name$='Respuesta3']").FillAsync("Respuesta tres");
        await Page.Locator("[name$='Respuesta4']").FillAsync("Respuesta cuatro");
        await Page.Locator("[name$='RespuestaCorrecta']").SelectOptionAsync("1");
        await Page.Locator("[name$='CategoriaId']").SelectOptionAsync(new SelectOptionValue { Label = "Arte" });
        await Page.GetByRole(AriaRole.Button, new() { NameRegex = new Regex("Crear|Guardar", RegexOptions.IgnoreCase) }).ClickAsync();
        await CerrarSweetAlertSiExisteAsync();
    }

    private Task AbrirPreguntaEnListadoAsync(string enunciado)
    {
        return AbrirAsync($"/Preguntas?Busqueda={Uri.EscapeDataString(enunciado)}");
    }
}
