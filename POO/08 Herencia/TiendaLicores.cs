public class TiendaLicores: Tienda
{
  public override void Bienvenida()
  {
    base.Bienvenida();
    Console.WriteLine("Si eres menor de 18 años, fuera de aquí.");
  }
}
