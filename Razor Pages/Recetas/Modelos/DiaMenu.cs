namespace Recetas.Modelos;

// Se usa un enum propio para mantener el orden de lunes a domingo.
public enum DiaMenu
{
    Lunes = 1,
    Martes = 2,
    Miercoles = 3,
    Jueves = 4,
    Viernes = 5,
    Sabado = 6,
    Domingo = 7
}

public static class DiaMenuExtensiones
{
    public static string Titulo(this DiaMenu dia) => dia switch
    {
        DiaMenu.Miercoles => "Miércoles",
        DiaMenu.Sabado => "Sábado",
        _ => dia.ToString()
    };
}
