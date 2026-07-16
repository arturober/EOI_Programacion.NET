using System.Text;

class TextoUtil
{
    public static string NormalizarParaComparar(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return string.Empty;
        }

        // Eliminar espacios en blanco al inicio y al final
        texto = texto.Trim();

        // Reemplazar múltiples espacios por un solo espacio
        texto = System.Text.RegularExpressions.Regex.Replace(texto, @"\s+", " ");

        StringBuilder sb = new StringBuilder();
        foreach (char c in texto)
        {
            sb.Append(NormalizarCaracter(c));
        }

        // Convertir a minúsculas y luego capitalizar la primera letra de cada palabra
        texto = sb.ToString().ToLower();

        return texto;
    }

    public static char NormalizarCaracter(char caracter)
    {
        // Convertir a minúscula
        char caracterNormalizado = char.ToLower(caracter);

        // Reemplazar caracteres acentuados por sus equivalentes sin acento
        switch (caracterNormalizado)
        {
            case 'á':
                return 'a';
            case 'é':
                return 'e';
            case 'í':
                return 'i';
            case 'ó':
                return 'o';
            case 'ú':
                return 'u';
            default:
                return caracterNormalizado;
        }
    }

    public static bool SonIguales(string texto1, string texto2)
    {
        return NormalizarParaComparar(texto1) == NormalizarParaComparar(texto2);
    }

    public static bool EsRespuestaValida(string respuesta)
    {
        if (string.IsNullOrWhiteSpace(respuesta))
        {
            return false;
        }

        // Verificar que la respuesta contenga solo letras y espacios
        foreach (char c in respuesta)
        {
            if (!char.IsLetter(c) && !char.IsWhiteSpace(c) && c != '-')
            {
                return false;
            }
        }

        return true;
    }

    public static bool EsLetraDelRosco(char letra)
    {
        string letras = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ";
        return letras.Contains(char.ToUpper(letra));
    }

    public static bool RespuestaContieneLetra(string respuesta, char letra)
    {
        string respuestaNormalizada = NormalizarParaComparar(respuesta);
        char letraNormalizada = NormalizarCaracter(letra);
        return respuestaNormalizada.Contains(letraNormalizada);
    }

    public static string LeerTextoObligatorio(string mensaje)
    {
        string? texto;
        do
        {
            Console.Write(mensaje);
            texto = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(texto))
            {
                Console.WriteLine("El texto no puede estar vacío. Inténtalo de nuevo.");
            }
        } while (string.IsNullOrWhiteSpace(texto));

        return texto;
    }

    public static int LeerEnteroPositivo(string mensaje)
    {
        int numero;
        do
        {
            Console.Write(mensaje);
            string? entrada = Console.ReadLine();
            if (!int.TryParse(entrada, out numero) || numero < 0)
            {
                Console.WriteLine("Por favor, introduce un número entero positivo.");
            }
        } while (numero < 0);

        return numero;
    }

    public static char LeerLetraRosco(string mensaje)
    {
        char letra;
        do
        {
            Console.Write(mensaje);
            string? entrada = Console.ReadLine()?.Trim().ToUpper();
            if (string.IsNullOrEmpty(entrada) || entrada.Length != 1 || !EsLetraDelRosco(entrada[0]))
            {
                Console.WriteLine("Por favor, introduce una letra válida del rosco (A-Z, incluyendo Ñ).");
                letra = '\0'; // Valor inválido
            }
            else
            {
                letra = entrada[0];
            }
        } while (letra == '\0');

        return letra;
    }

    public static bool Confirmar(string mensaje)
    {
        Console.Write(mensaje + " (s/n): ");
        string? respuesta = Console.ReadLine()?.Trim().ToLower();
        return respuesta == "s" || respuesta == "si" || respuesta == "sí";
    }

    public static void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
    }
}