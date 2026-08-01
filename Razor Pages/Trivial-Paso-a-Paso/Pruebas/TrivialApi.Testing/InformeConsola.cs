namespace TrivialApi.Testing;

// Centraliza los mensajes que se muestran durante las pruebas.
// Los colores ANSI son opcionales porque algunos runners muestran sus códigos
// como texto. Para activarlos se utiliza TRIVIAL_TEST_COLORS=1.
public sealed class InformeConsola(Action<string> escribir)
{
    private const string Restablecer = "\u001b[0m";
    private const string Azul = "\u001b[94m";
    private const string Cian = "\u001b[96m";
    private const string Amarillo = "\u001b[93m";
    private const string Verde = "\u001b[92m";
    private const string Gris = "\u001b[90m";

    private static readonly bool UsarColores =
        Environment.GetEnvironmentVariable("NO_COLOR") is null &&
        Environment.GetEnvironmentVariable("TRIVIAL_TEST_COLORS") == "1";

    public void Inicio(string descripcion)
    {
        escribir(string.Empty);
        escribir(Colorear(Azul, new string('=', 74)));
        escribir(Colorear(Azul, $"INICIO: {descripcion}"));
    }

    public void Paso(string descripcion) =>
        escribir(Colorear(Cian, $"PASO: {descripcion}"));

    public void Peticion(string metodo, string ruta) =>
        escribir(Colorear(Amarillo, $"PETICIÓN: {metodo} {ruta}"));

    public void Respuesta(int codigo, string descripcion) =>
        escribir(Colorear(Gris, $"RESPUESTA: {codigo} {descripcion}"));

    public void Comprobacion(string descripcion) =>
        escribir(Colorear(Cian, $"COMPROBACIÓN: {descripcion}"));

    public void Exito() =>
        escribir(Colorear(Verde, "RESULTADO: prueba superada"));

    private static string Colorear(string color, string texto)
    {
        return UsarColores ? $"{color}{texto}{Restablecer}" : texto;
    }
}
