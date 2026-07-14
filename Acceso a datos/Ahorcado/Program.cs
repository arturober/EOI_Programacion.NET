class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        BaseDatos baseDatos = new BaseDatos();
        baseDatos.Preparar();
    }
}