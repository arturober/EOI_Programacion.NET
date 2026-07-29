namespace OpenWeather.Modelos;

// Reúne los dos sistemas de unidades que ofrece la interfaz.
public enum Unidades
{
    Metrico,
    Imperial
}

public static class UnidadesExtensiones
{
    // Convierte el valor de la URL al tipo enumerado de la aplicación.
    public static Unidades DesdeTexto(string? texto)
    {
        return texto?.Equals("imperial", StringComparison.OrdinalIgnoreCase) == true
            ? Unidades.Imperial
            : Unidades.Metrico;
    }

    // Devuelve el texto que espera el parámetro units de OpenWeather.
    public static string ParaApi(this Unidades unidades)
    {
        return unidades == Unidades.Imperial ? "imperial" : "metric";
    }

    // Devuelve el texto breve que se empleará en las direcciones de esta aplicación.
    public static string ParaUrl(this Unidades unidades)
    {
        return unidades == Unidades.Imperial ? "imperial" : "metrico";
    }

    // Devuelve el símbolo que acompaña a las temperaturas.
    public static string SimboloTemperatura(this Unidades unidades)
    {
        return unidades == Unidades.Imperial ? "°F" : "°C";
    }

    // En el sistema métrico se mostrarán kilómetros por hora.
    public static string UnidadViento(this Unidades unidades)
    {
        return unidades == Unidades.Imperial ? "mph" : "km/h";
    }
}
