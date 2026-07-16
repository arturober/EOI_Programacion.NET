using System.Text;

class TextoUtil
{
    // Convierte un texto a una forma sencilla de comparar:
    // - elimina espacios repetidos;
    // - utiliza minúsculas;
    // - considera equivalentes las vocales con y sin tilde;
    // - conserva la ñ, porque es una letra diferente de la n.
    public static string NormalizarParaComparar(string texto)
    {
        string textoSinEspaciosExtra = string.Join(
            " ",
            texto.Trim().Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries));

        StringBuilder resultado = new StringBuilder();

        foreach (char caracter in textoSinEspaciosExtra)
        {
            resultado.Append(NormalizarCaracter(caracter));
        }

        return resultado.ToString().ToLowerInvariant();
    }

    public static char NormalizarCaracter(char caracter)
    {
        char minuscula = char.ToLowerInvariant(caracter);
        char resultado;

        switch (minuscula)
        {
            case 'á':
                resultado = 'a';
                break;

            case 'é':
                resultado = 'e';
                break;

            case 'í':
                resultado = 'i';
                break;

            case 'ó':
                resultado = 'o';
                break;

            case 'ú':
            case 'ü':
                resultado = 'u';
                break;

            default:
                resultado = minuscula;
                break;
        }

        return resultado;
    }

    public static bool SonIguales(
        string texto1,
        string texto2)
    {
        string texto1Normalizado =
            NormalizarParaComparar(texto1);
        string texto2Normalizado =
            NormalizarParaComparar(texto2);

        return texto1Normalizado == texto2Normalizado;
    }

    // Admitimos letras, espacios y guiones. De este modo se pueden guardar
    // tanto palabras como expresiones sencillas: "ciencia ficción".
    public static bool EsPalabraValida(string texto)
    {
        bool contieneLetra = false;
        bool esValida = true;

        foreach (char caracter in texto)
        {
            if (char.IsLetter(caracter))
            {
                contieneLetra = true;
            }
            else if (caracter != ' ' && caracter != '-')
            {
                esValida = false;
            }
        }

        return esValida && contieneLetra;
    }

    // Los siguientes métodos estaban antes en EntradaConsola.cs.
    // Se han trasladado aquí para reducir el número de clases del proyecto.

    public static string LeerTextoObligatorio(string mensaje)
    {
        string texto = "";

        while (texto == "")
        {
            Console.Write(mensaje);
            texto = (Console.ReadLine() ?? "").Trim();

            if (texto == "")
            {
                Console.WriteLine("El texto no puede estar vacío.");
            }
        }

        return texto;
    }

    public static int LeerEnteroPositivo(string mensaje)
    {
        int numero = 0;
        bool numeroValido = false;

        while (!numeroValido)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine() ?? "";

            bool esNumero = int.TryParse(texto, out numero);
            numeroValido = esNumero && numero > 0;

            if (!numeroValido)
            {
                Console.WriteLine(
                    "Debes introducir un número entero mayor que cero.");
            }
        }

        return numero;
    }

    public static bool Confirmar(string mensaje)
    {
        Console.Write(mensaje + " (s/n): ");
        string respuesta = (Console.ReadLine() ?? "")
            .Trim()
            .ToLower();

        bool confirmada =
            respuesta == "s" ||
            respuesta == "sí" ||
            respuesta == "si";

        return confirmada;
    }

    public static void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Pulsa cualquier tecla para continuar...");
        Console.ReadKey(true);
    }
}
