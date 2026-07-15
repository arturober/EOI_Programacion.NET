class TextoUtil
{
    public static string LeerTextoObligatorio(string mensaje)
    {
        string texto = "";

        while (texto == "")
        {
            Console.Write(mensaje);
            texto = (Console.ReadLine() ?? "").Trim();
            if (texto == "")
            {
                Console.WriteLine("El campo no puede estar vacío. Inténtalo de nuevo.");
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
            string input = Console.ReadLine() ?? "";

            bool esNumero = int.TryParse(input, out numero);
            numeroValido = esNumero && numero > 0;

            if (!numeroValido)
            {
                Console.WriteLine("Por favor, introduzca un número entero positivo.");
            }
        }

        return numero;
    }

    public static bool Confirmar(string mensaje)
    {
        Console.Write(mensaje + " (s/n): ");
        string respuesta = (Console.ReadLine() ?? "").Trim().ToLower();
        return respuesta == "s";
    }

    public static void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey(true);
    }
}