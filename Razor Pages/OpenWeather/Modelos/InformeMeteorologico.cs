namespace OpenWeather.Modelos;

// Agrupa todos los datos necesarios para dibujar la página de resultados.
public class InformeMeteorologico
{
    public required Lugar Lugar { get; set; }
    public required TiempoActual Actual { get; set; }
    public required IReadOnlyList<PrevisionPeriodo> ProximasHoras { get; set; }
    public required IReadOnlyList<PrevisionDiaria> ProximosDias { get; set; }
    public CalidadAire? Aire { get; set; }
    public required Unidades Unidades { get; set; }

    public string SimboloTemperatura => Unidades.SimboloTemperatura();
    public string UnidadViento => Unidades.UnidadViento();
}

// Representa las condiciones meteorológicas del momento actual.
public class TiempoActual
{
    public DateTimeOffset Fecha { get; set; }
    public string Descripcion { get; set; } = "";
    public string Icono { get; set; } = "01d";
    public double Temperatura { get; set; }
    public double Sensacion { get; set; }
    public int Humedad { get; set; }
    public int Presion { get; set; }
    public int VisibilidadMetros { get; set; }
    public int Nubosidad { get; set; }
    public double Viento { get; set; }
    public double? Racha { get; set; }
    public int DireccionViento { get; set; }
    public DateTimeOffset Amanecer { get; set; }
    public DateTimeOffset Atardecer { get; set; }

    // La API devuelve la visibilidad en metros y la interfaz la muestra en kilómetros.
    public double VisibilidadKilometros => VisibilidadMetros / 1000d;

    // Convierte los grados de la API en una dirección fácil de interpretar.
    public string DireccionVientoTexto => UtilidadesMeteorologicas.DireccionCardinal(DireccionViento);

    // La dirección se genera en un método común para no repetirla.
    public string UrlIcono => UtilidadesMeteorologicas.UrlIcono(Icono);
}

// Representa uno de los intervalos de tres horas de la previsión gratuita.
public class PrevisionPeriodo
{
    public DateTimeOffset Fecha { get; set; }
    public string Descripcion { get; set; } = "";
    public string Icono { get; set; } = "01d";
    public double Temperatura { get; set; }
    public double Sensacion { get; set; }
    public int Humedad { get; set; }
    public double ProbabilidadLluvia { get; set; }
    public double LluviaMilimetros { get; set; }
    public double Viento { get; set; }

    public string UrlIcono => UtilidadesMeteorologicas.UrlIcono(Icono);
}

// Resume los intervalos de un mismo día para facilitar su lectura.
public class PrevisionDiaria
{
    public DateOnly Fecha { get; set; }
    public string Descripcion { get; set; } = "";
    public string Icono { get; set; } = "01d";
    public double Minima { get; set; }
    public double Maxima { get; set; }
    public int Humedad { get; set; }
    public double ProbabilidadLluvia { get; set; }
    public double LluviaMilimetros { get; set; }
    public double VientoMaximo { get; set; }

    public string UrlIcono => UtilidadesMeteorologicas.UrlIcono(Icono);
}

// Contiene el índice general y las concentraciones de contaminantes.
public class CalidadAire
{
    public int Indice { get; set; }
    public DateTimeOffset Fecha { get; set; }
    public double MonoxidoCarbono { get; set; }
    public double MonoxidoNitrogeno { get; set; }
    public double DioxidoNitrogeno { get; set; }
    public double Ozono { get; set; }
    public double DioxidoAzufre { get; set; }
    public double Pm25 { get; set; }
    public double Pm10 { get; set; }
    public double Amoniaco { get; set; }

    // La escala de OpenWeather utiliza valores del 1 al 5.
    public string Descripcion => Indice switch
    {
        1 => "Buena",
        2 => "Aceptable",
        3 => "Moderada",
        4 => "Mala",
        5 => "Muy mala",
        _ => "Sin datos"
    };

    // Estas clases de Bootstrap permiten reconocer rápidamente el nivel.
    public string ClaseBootstrap => Indice switch
    {
        1 => "success",
        2 => "info",
        3 => "warning",
        4 => "danger",
        5 => "dark",
        _ => "secondary"
    };
}

public static class UtilidadesMeteorologicas
{
    // OpenWeather devuelve códigos como 01d o 10n.
    // La carpeta img/wn sigue sirviendo correctamente los iconos PNG.
    public static string UrlIcono(string icono)
    {
        return $"https://openweathermap.org/img/wn/{icono}@2x.png";
    }

    // Divide la rosa de los vientos en ocho sectores de 45 grados.
    public static string DireccionCardinal(int grados)
    {
        string[] direcciones = ["N", "NE", "E", "SE", "S", "SO", "O", "NO"];
        int indice = (int)Math.Round(grados / 45d, MidpointRounding.AwayFromZero) % 8;
        return direcciones[indice];
    }

    // Pone en mayúscula solamente la primera letra de una descripción.
    public static string PrimeraMayuscula(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return "";
        }

        return char.ToUpper(texto[0]) + texto[1..];
    }
}
