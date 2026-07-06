using System.Numerics;
using Raylib_cs;

// Inicialización de la ventana
const int anchoPantalla = 800;
const int altoPantalla = 600;
Raylib.InitWindow(anchoPantalla, altoPantalla, "Super Shooter 2D - Practicando POO");
Raylib.SetTargetFPS(60); // Capamos a 60 FPS
Raylib.HideCursor();     // Ocultamos el cursor del sistema para dibujar una mira personalizada

// Estado del juego
var jugador = new Jugador(new Vector2(400, 300), 300.0f);
var enemigos = new List<Enemigo>();
var proyectiles = new List<Proyectil>();
var particulas = new List<Particula>();

int puntuacion = 0;
bool juegoTerminado = false;
float tiempoTranscurrido = 0f;

// Temporizadores para generación de enemigos
float temporizadorSpawn = 0f;
float intervaloSpawnBase = 1.5f;

// Configuración de la cámara para efecto de sacudida (Screen Shake)
var camara = new Camera2D
{
  Target = Vector2.Zero,
  Offset = Vector2.Zero,
  Rotation = 0f,
  Zoom = 1.0f
};
float tiempoSacudida = 0f;
float intensidadSacudida = 6f;

// Función auxiliar para crear explosiones de partículas
void CrearExplosion(Vector2 posicion, Color color, int cantidad)
{
  for (int i = 0; i < cantidad; i++)
  {
    float angulo = Random.Shared.NextSingle() * MathF.PI * 2f;
    float velocidad = (Random.Shared.NextSingle() * 180f) + 40f;
    Vector2 vel = new Vector2(MathF.Cos(angulo), MathF.Sin(angulo)) * velocidad;
    float duracion = (Random.Shared.NextSingle() * 0.5f) + 0.2f;
    particulas.Add(new Particula(posicion, vel, color, duracion));
  }
}

// Función para reiniciar el juego
void ReiniciarJuego()
{
  jugador = new Jugador(new Vector2(400, 300), 300.0f);
  enemigos.Clear();
  proyectiles.Clear();
  particulas.Clear();
  puntuacion = 0;
  tiempoTranscurrido = 0f;
  juegoTerminado = false;
  temporizadorSpawn = 0f;
}

// EL BUCLE PRINCIPAL
while (!Raylib.WindowShouldClose())
{
  float deltaTime = Raylib.GetFrameTime();

  if (!juegoTerminado)
  {
    tiempoTranscurrido += deltaTime;

    // --- 1. ENTRADA Y LÓGICA DEL JUGADOR ---
    jugador.Actualizar(deltaTime);

    // Disparo (Apuntar al ratón)
    if (Raylib.IsMouseButtonDown(MouseButton.Left) || Raylib.IsKeyDown(KeyboardKey.Space))
    {
      Vector2 posicionRaton = Raylib.GetMousePosition();
      Vector2 centroJugador = jugador.Posicion + jugador.Tamano / 2;
      Vector2 direccionDisparo = posicionRaton - centroJugador;

      if (direccionDisparo == Vector2.Zero)
      {
        direccionDisparo = new Vector2(0, -1); // Por defecto hacia arriba si se hace clic encima
      }
      else
      {
        direccionDisparo = Vector2.Normalize(direccionDisparo);
      }

      Proyectil? nuevoProyectil = jugador.IntentarDisparar(direccionDisparo);
      if (nuevoProyectil != null)
      {
        proyectiles.Add(nuevoProyectil);
        // Pequeño efecto visual de destello de disparo
        CrearExplosion(centroJugador + direccionDisparo * 15f, new Color(255, 255, 150, 200), 2);
      }
    }

    // --- 2. GENERACIÓN DE ENEMIGOS ---
    temporizadorSpawn += deltaTime;
    // Reducir la frecuencia de spawn gradualmente conforme avanza el tiempo
    float intervaloActual = Math.Max(0.5f, intervaloSpawnBase - (tiempoTranscurrido / 45f) * 0.25f);

    if (temporizadorSpawn >= intervaloActual)
    {
      temporizadorSpawn = 0f;

      // Elegir un borde aleatorio para spawnear al enemigo (fuera de pantalla)
      Vector2 posSpawn = Vector2.Zero;
      int borde = Random.Shared.Next(0, 4);
      float margen = 40f;

      switch (borde)
      {
        case 0: // Arriba
          posSpawn = new Vector2(Random.Shared.NextSingle() * anchoPantalla, -margen);
          break;
        case 1: // Abajo
          posSpawn = new Vector2(Random.Shared.NextSingle() * anchoPantalla, altoPantalla + margen);
          break;
        case 2: // Izquierda
          posSpawn = new Vector2(-margen, Random.Shared.NextSingle() * altoPantalla);
          break;
        case 3: // Derecha
          posSpawn = new Vector2(anchoPantalla + margen, Random.Shared.NextSingle() * altoPantalla);
          break;
      }

      // 70% enemigo básico, 30% enemigo rápido
      if (Random.Shared.NextSingle() < 0.7f)
      {
        enemigos.Add(Enemigo.CrearEnemigoBasico(posSpawn, jugador));
      }
      else
      {
        enemigos.Add(Enemigo.CrearEnemigoRapido(posSpawn, jugador));
      }
    }

    // --- 3. ACTUALIZACIÓN DE ENTIDADES ---
    foreach (var enemigo in enemigos) enemigo.Actualizar(deltaTime);
    foreach (var proyectil in proyectiles) proyectil.Actualizar(deltaTime);
    foreach (var particula in particulas) particula.Actualizar(deltaTime);

    // --- 4. GESTIÓN DE COLISIONES ---
    // Proyectil contra Enemigos
    foreach (var p in proyectiles)
    {
      if (!p.Activo) continue;

      foreach (var e in enemigos)
      {
        if (!e.Activo) continue;

        if (p.ColisionaCon(e))
        {
          p.Activo = false;
          e.RecibirDano(10); // Cada bala hace 10 de daño

          // Partículas de impacto
          CrearExplosion(p.Posicion, e.Color, 4);

          if (!e.Activo)
          {
            // Enemigo destruido
            puntuacion += e.PuntosAlMorir;
            CrearExplosion(e.Posicion + e.Tamano / 2, e.Color, 12);
          }
          break; // Romper bucle de enemigos para este proyectil
        }
      }
    }

    // Enemigos contra Jugador
    foreach (var e in enemigos)
    {
      if (!e.Activo) continue;

      if (e.ColisionaCon(jugador))
      {
        jugador.RecibirDano(e.DanoAlColisionar);
        e.Activo = false; // El enemigo se inmola al tocar al jugador

        // Explosión por colisión
        CrearExplosion(e.Posicion + e.Tamano / 2, e.Color, 8);

        // Activar sacudida de pantalla
        tiempoSacudida = 0.25f;

        if (!jugador.Activo)
        {
          juegoTerminado = true;
          // Gran explosión del jugador
          CrearExplosion(jugador.Posicion + jugador.Tamano / 2, jugador.Color, 30);
        }
      }
    }

    // Limpieza de entidades inactivas
    enemigos.RemoveAll(e => !e.Activo);
    proyectiles.RemoveAll(p => !p.Activo);
    particulas.RemoveAll(part => !part.Activo);
  }
  else
  {
    // Si el juego ha terminado, presionar 'R' para reiniciar
    if (Raylib.IsKeyPressed(KeyboardKey.R))
    {
      ReiniciarJuego();
    }
  }

  // --- 5. ACTUALIZACIÓN DE SACUDIDA DE PANTALLA ---
  if (tiempoSacudida > 0f)
  {
    tiempoSacudida -= deltaTime;
    camara.Offset = new Vector2(
        (Random.Shared.NextSingle() * 2f - 1f) * intensidadSacudida,
        (Random.Shared.NextSingle() * 2f - 1f) * intensidadSacudida
    );
  }
  else
  {
    camara.Offset = Vector2.Zero;
  }

  // --- 6. RENDERIZADO (DIBUJO EN PANTALLA) ---
  Raylib.BeginDrawing();
  Raylib.ClearBackground(new Color(20, 24, 33, 255)); // Fondo azul oscuro moderno/futurista

  // Comenzar modo cámara (afecta a los elementos del juego para la sacudida)
  Raylib.BeginMode2D(camara);

  // Dibujar proyectiles
  foreach (var proyectil in proyectiles) proyectil.Dibujar();

  // Dibujar enemigos
  foreach (var enemigo in enemigos) enemigo.Dibujar();

  // Dibujar partículas
  foreach (var particula in particulas) particula.Dibujar();

  // Dibujar jugador (si está vivo)
  if (jugador.Activo)
  {
    jugador.Dibujar();
  }

  Raylib.EndMode2D();

  // --- 7. DIBUJAR INTERFAZ DE USUARIO (HUD) ---
  // Barra de vida del Jugador (Esquina superior izquierda)
  Raylib.DrawText("VIDA:", 20, 20, 20, Color.LightGray);

  // Contenedor de barra de vida
  Raylib.DrawRectangle(90, 18, 204, 24, Color.DarkGray);
  Raylib.DrawRectangle(92, 20, 200, 20, new Color(40, 45, 55, 255));

  // Relleno de vida proporcional (rojo/verde fluido)
  float ratioVida = (float)jugador.Vida / jugador.VidaMaxima;
  Color colorVida = ratioVida > 0.4f ? Color.Green : Color.Red;
  Raylib.DrawRectangle(92, 20, (int)(200 * ratioVida), 20, colorVida);

  // Texto de vida exacta
  Raylib.DrawText($"{jugador.Vida}/{jugador.VidaMaxima}", 160, 22, 16, Color.White);

  // Puntuación (Esquina superior derecha)
  string textoScore = $"SCORE: {puntuacion}";
  int anchoTexto = Raylib.MeasureText(textoScore, 20);
  Raylib.DrawText(textoScore, anchoPantalla - anchoTexto - 20, 20, 20, Color.Gold);

  // Si el juego ha terminado, dibujar pantalla de Game Over
  if (juegoTerminado)
  {
    // Fondo oscurecido semitransparente
    Raylib.DrawRectangle(0, 0, anchoPantalla, altoPantalla, new Color(10, 10, 15, 200));

    const string msgGameOver = "GAME OVER";
    int w1 = Raylib.MeasureText(msgGameOver, 50);
    Raylib.DrawText(msgGameOver, (anchoPantalla - w1) / 2, altoPantalla / 2 - 80, 50, Color.Red);

    string msgScore = $"Puntuación Final: {puntuacion}";
    int w2 = Raylib.MeasureText(msgScore, 25);
    Raylib.DrawText(msgScore, (anchoPantalla - w2) / 2, altoPantalla / 2 - 10, 25, Color.White);

    const string msgRestart = "Presiona 'R' para reiniciar o 'ESC' para salir";
    int w3 = Raylib.MeasureText(msgRestart, 20);
    Raylib.DrawText(msgRestart, (anchoPantalla - w3) / 2, altoPantalla / 2 + 40, 20, Color.LightGray);
  }

  // Dibujar mira personalizada en la posición del ratón
  Vector2 mousePos = Raylib.GetMousePosition();
  Color colorMira = juegoTerminado ? Color.DarkGray : Color.SkyBlue;
  Raylib.DrawCircleLines((int)mousePos.X, (int)mousePos.Y, 8, colorMira);
  Raylib.DrawLine((int)mousePos.X - 12, (int)mousePos.Y, (int)mousePos.X + 12, (int)mousePos.Y, colorMira);
  Raylib.DrawLine((int)mousePos.X, (int)mousePos.Y - 12, (int)mousePos.X, (int)mousePos.Y + 12, colorMira);

  Raylib.EndDrawing();
}

// Restablecer el cursor al salir
Raylib.ShowCursor();

// Cierre limpio de la ventana
Raylib.CloseWindow();

