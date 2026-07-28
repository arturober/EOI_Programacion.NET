public class Partida
{
    public List<PreguntaPartida> Preguntas { get; set; } = new List<PreguntaPartida>();
    public int IndiceActual { get; set; }
    public bool Terminada { get; set; }

    public PreguntaPartida PreguntaActual
    {
        get { return Preguntas[IndiceActual]; }
    }

    public int Aciertos
    {
        get { return Preguntas.Count(p => p.Estado == "correcta"); }
    }

    public int Fallos
    {
        get { return Preguntas.Count(p => p.Estado == "incorrecta"); }
    }

    public int Pendientes
    {
        get { return Preguntas.Count(p => p.Estado == "pendiente"); }
    }

    public void Avanzar()
    {
        if (Pendientes == 0)
        {
            Terminada = true;
            return;
        }

        do
        {
            IndiceActual++;

            if (IndiceActual == Preguntas.Count)
            {
                IndiceActual = 0;
            }
        }
        while (PreguntaActual.Estado != "pendiente");
    }
}

public class PreguntaPartida
{
    public int Id { get; set; }
    public char Letra { get; set; }
    public string Enunciado { get; set; } = "";
    public string Respuesta { get; set; } = "";
    public string Tema { get; set; } = "";
    public string Estado { get; set; } = "pendiente";
}
