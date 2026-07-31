using System.Text;
using System.Text.Json;
using Godot;

// La escena solo contiene este Control.
// Todos los controles de la interfaz se crean desde este mismo script.
public partial class JuegoTrivial : Control
{
    // HttpRequest permite realizar peticiones HTTP sin paquetes adicionales.
    private HttpRequest _http = null!;

    // Controles de la pantalla inicial.
    private VBoxContainer _pantallaInicio = null!;
    private LineEdit _entradaUrl = null!;
    private OptionButton _selectorCategoria = null!;
    private Button _botonConectar = null!;
    private Button _botonComenzar = null!;
    private Label _estado = null!;

    // Controles utilizados mientras se responde una partida.
    private VBoxContainer _pantallaJuego = null!;
    private Label _progreso = null!;
    private Label _nombreCategoria = null!;
    private Label _enunciado = null!;
    private VBoxContainer _contenedorRespuestas = null!;
    private Label _resultadoRespuesta = null!;
    private Button _botonSiguiente = null!;

    // Controles de la pantalla final.
    private VBoxContainer _pantallaFinal = null!;
    private Label _resultadoFinal = null!;

    // Estado de la partida actual.
    private List<PreguntaDto> _preguntas = [];
    private int _posicion;
    private int _aciertos;

    // La opción hace que System.Text.Json acepte propiedades como "nombre"
    // aunque la propiedad correspondiente del DTO se llame "Nombre".
    private readonly JsonSerializerOptions _opcionesJson = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public override void _Ready()
    {
        // Construimos primero la interfaz. De este modo la pantalla inicial
        // no depende de que se haya realizado todavía ninguna petición HTTP.
        CrearInterfaz();

        // Este nodo se ocupa únicamente de las peticiones HTTP.
        // El tiempo máximo evita que la interfaz espere indefinidamente
        // cuando la dirección existe pero el servidor no responde.
        _http = new HttpRequest
        {
            Timeout = 10
        };
        AddChild(_http);
    }

    private void CrearInterfaz()
    {
        // ColorRect crea el fondo sin utilizar ninguna imagen ni recurso externo.
        ColorRect fondo = new()
        {
            Color = new Color(0.055f, 0.075f, 0.12f),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };
        AddChild(fondo);
        fondo.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        // MarginContainer mantiene el contenido separado de los bordes.
        MarginContainer margenExterior = new();
        AddChild(margenExterior);
        margenExterior.SetAnchorsAndOffsetsPreset(
            Control.LayoutPreset.FullRect);
        margenExterior.AddThemeConstantOverride("margin_left", 24);
        margenExterior.AddThemeConstantOverride("margin_top", 24);
        margenExterior.AddThemeConstantOverride("margin_right", 24);
        margenExterior.AddThemeConstantOverride("margin_bottom", 24);

        // ScrollContainer permite utilizar la aplicación en ventanas pequeñas.
        ScrollContainer desplazamiento = new()
        {
            HorizontalScrollMode =
                ScrollContainer.ScrollMode.Disabled,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        margenExterior.AddChild(desplazamiento);

        // PanelContainer representa la tarjeta principal de la aplicación.
        PanelContainer tarjeta = new()
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        tarjeta.AddThemeStyleboxOverride(
            "panel",
            CrearEstiloTarjeta());
        desplazamiento.AddChild(tarjeta);

        // Este segundo margen separa los controles del borde de la tarjeta.
        MarginContainer margenInterior = new();
        margenInterior.AddThemeConstantOverride("margin_left", 28);
        margenInterior.AddThemeConstantOverride("margin_top", 28);
        margenInterior.AddThemeConstantOverride("margin_right", 28);
        margenInterior.AddThemeConstantOverride("margin_bottom", 28);
        tarjeta.AddChild(margenInterior);

        VBoxContainer contenido = new()
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        contenido.AddThemeConstantOverride("separation", 16);
        margenInterior.AddChild(contenido);

        Label titulo = CrearEtiqueta(
            "Cliente Godot del Trivial",
            36,
            HorizontalAlignment.Center);
        contenido.AddChild(titulo);

        Label descripcion = CrearEtiqueta(
            "Cliente programado completamente desde un único Control.",
            18,
            HorizontalAlignment.Center);
        descripcion.Modulate = new Color(0.75f, 0.8f, 0.9f);
        contenido.AddChild(descripcion);

        contenido.AddChild(new HSeparator());

        CrearPantallaInicio(contenido);
        CrearPantallaJuego(contenido);
        CrearPantallaFinal(contenido);
    }

    private void CrearPantallaInicio(VBoxContainer contenido)
    {
        _pantallaInicio = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _pantallaInicio.AddThemeConstantOverride("separation", 12);
        contenido.AddChild(_pantallaInicio);

        _pantallaInicio.AddChild(
            CrearEtiqueta("Dirección del servidor", 18));

        // Puede cambiarse la dirección sin modificar ni recompilar el código.
        _entradaUrl = new LineEdit
        {
            Text = "http://localhost:5000",
            PlaceholderText = "http://localhost:5000",
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _entradaUrl.AddThemeFontSizeOverride("font_size", 18);
        _pantallaInicio.AddChild(_entradaUrl);

        _botonConectar = CrearBoton("Conectar y cargar categorías");
        _botonConectar.Pressed += async () =>
            await CargarCategoriasAsync();
        _pantallaInicio.AddChild(_botonConectar);

        _pantallaInicio.AddChild(
            CrearEtiqueta("Categoría", 18));

        _selectorCategoria = new OptionButton
        {
            Disabled = true,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _selectorCategoria.AddThemeFontSizeOverride("font_size", 18);
        _pantallaInicio.AddChild(_selectorCategoria);

        _botonComenzar = CrearBoton("Comenzar partida");
        _botonComenzar.Disabled = true;
        _botonComenzar.Pressed += async () =>
            await ComenzarPartidaAsync();
        _pantallaInicio.AddChild(_botonComenzar);

        _estado = CrearEtiqueta("", 17);
        _estado.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _pantallaInicio.AddChild(_estado);
    }

    private void CrearPantallaJuego(VBoxContainer contenido)
    {
        _pantallaJuego = new VBoxContainer
        {
            Visible = false,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _pantallaJuego.AddThemeConstantOverride("separation", 12);
        contenido.AddChild(_pantallaJuego);

        _progreso = CrearEtiqueta(
            "",
            18,
            HorizontalAlignment.Right);
        _pantallaJuego.AddChild(_progreso);

        _nombreCategoria = CrearEtiqueta("", 18);
        _nombreCategoria.Modulate =
            new Color(0.5f, 0.75f, 1.0f);
        _pantallaJuego.AddChild(_nombreCategoria);

        _enunciado = CrearEtiqueta("", 26);
        _enunciado.AutowrapMode =
            TextServer.AutowrapMode.WordSmart;
        _pantallaJuego.AddChild(_enunciado);

        _contenedorRespuestas = new VBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _contenedorRespuestas.AddThemeConstantOverride(
            "separation",
            10);
        _pantallaJuego.AddChild(_contenedorRespuestas);

        _resultadoRespuesta = CrearEtiqueta("", 19);
        _resultadoRespuesta.AutowrapMode =
            TextServer.AutowrapMode.WordSmart;
        _pantallaJuego.AddChild(_resultadoRespuesta);

        _botonSiguiente = CrearBoton("Siguiente pregunta");
        _botonSiguiente.Visible = false;
        _botonSiguiente.Pressed += MostrarSiguientePregunta;
        _pantallaJuego.AddChild(_botonSiguiente);
    }

    private void CrearPantallaFinal(VBoxContainer contenido)
    {
        _pantallaFinal = new VBoxContainer
        {
            Visible = false,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        _pantallaFinal.AddThemeConstantOverride("separation", 18);
        contenido.AddChild(_pantallaFinal);

        _pantallaFinal.AddChild(
            CrearEtiqueta(
                "Partida terminada",
                30,
                HorizontalAlignment.Center));

        _resultadoFinal = CrearEtiqueta(
            "",
            24,
            HorizontalAlignment.Center);
        _pantallaFinal.AddChild(_resultadoFinal);

        Button botonOtraPartida = CrearBoton("Jugar otra partida");
        botonOtraPartida.Pressed += VolverAlInicio;
        _pantallaFinal.AddChild(botonOtraPartida);
    }

    private async Task CargarCategoriasAsync()
    {
        _botonConectar.Disabled = true;
        _botonComenzar.Disabled = true;
        _selectorCategoria.Disabled = true;
        MostrarEstado("Conectando con la API...", false);

        try
        {
            List<CategoriaDto>? categorias =
                await ObtenerJsonAsync<List<CategoriaDto>>(
                    "categorias");

            if (categorias is null || categorias.Count == 0)
            {
                MostrarEstado(
                    "La API no ha devuelto categorías.",
                    true);
                return;
            }

            // Limpiamos las opciones por si se pulsa Conectar varias veces.
            _selectorCategoria.Clear();
            _selectorCategoria.AddItem(
                "Todas las categorías",
                0);

            foreach (CategoriaDto categoria in categorias)
            {
                // El Id de cada opción coincide con el Id de la base de datos.
                _selectorCategoria.AddItem(
                    categoria.Nombre,
                    categoria.Id);
            }

            _selectorCategoria.Disabled = false;
            _botonComenzar.Disabled = false;
            MostrarEstado(
                $"Conexión correcta: {categorias.Count} categorías.",
                false);
        }
        catch (Exception error)
        {
            MostrarEstado(
                $"No se ha podido consultar la API: {error.Message}",
                true);
        }
        finally
        {
            _botonConectar.Disabled = false;
        }
    }

    private async Task ComenzarPartidaAsync()
    {
        _botonComenzar.Disabled = true;
        MostrarEstado("Descargando las preguntas...", false);

        try
        {
            int categoriaId = _selectorCategoria.GetSelectedId();

            string ruta = categoriaId == 0
                ? "preguntas?cantidad=10"
                : $"preguntas?categoriaId={categoriaId}&cantidad=10";

            _preguntas =
                await ObtenerJsonAsync<List<PreguntaDto>>(ruta)
                ?? [];

            if (_preguntas.Count == 0)
            {
                MostrarEstado(
                    "La categoría seleccionada no contiene preguntas.",
                    true);
                return;
            }

            _posicion = 0;
            _aciertos = 0;

            _pantallaInicio.Visible = false;
            _pantallaFinal.Visible = false;
            _pantallaJuego.Visible = true;

            MostrarPregunta();
        }
        catch (Exception error)
        {
            MostrarEstado(
                $"No se han podido cargar las preguntas: {error.Message}",
                true);
        }
        finally
        {
            _botonComenzar.Disabled = false;
        }
    }

    private void MostrarPregunta()
    {
        PreguntaDto pregunta = _preguntas[_posicion];

        _progreso.Text =
            $"Pregunta {_posicion + 1} de {_preguntas.Count} · " +
            $"{_aciertos} aciertos";
        _nombreCategoria.Text =
            $"Categoría: {pregunta.Categoria.Nombre}";
        _enunciado.Text = pregunta.Enunciado;
        _resultadoRespuesta.Text = "";
        _botonSiguiente.Visible = false;

        // Eliminamos los botones pertenecientes a la pregunta anterior.
        foreach (Node hijo in _contenedorRespuestas.GetChildren())
        {
            _contenedorRespuestas.RemoveChild(hijo);
            hijo.QueueFree();
        }

        for (int indice = 0;
             indice < pregunta.Respuestas.Length;
             indice++)
        {
            int numeroRespuesta = indice + 1;

            Button boton = CrearBoton(
                $"{numeroRespuesta}. {pregunta.Respuestas[indice]}");
            boton.Alignment = HorizontalAlignment.Left;
            boton.Pressed += () =>
                Responder(numeroRespuesta);

            _contenedorRespuestas.AddChild(boton);
        }
    }

    private void Responder(int numeroRespuesta)
    {
        PreguntaDto pregunta = _preguntas[_posicion];
        bool esCorrecta =
            numeroRespuesta == pregunta.RespuestaCorrecta;

        // Desactivamos todos los botones para impedir una segunda respuesta.
        foreach (Node hijo in _contenedorRespuestas.GetChildren())
        {
            if (hijo is Button boton)
            {
                boton.Disabled = true;
            }
        }

        if (esCorrecta)
        {
            _aciertos++;
            _resultadoRespuesta.Text = "¡Respuesta correcta!";
            _resultadoRespuesta.Modulate =
                new Color(0.35f, 0.9f, 0.5f);
        }
        else
        {
            string respuestaCorrecta =
                pregunta.Respuestas[
                    pregunta.RespuestaCorrecta - 1];

            _resultadoRespuesta.Text =
                $"Respuesta incorrecta. La correcta era: " +
                respuestaCorrecta;
            _resultadoRespuesta.Modulate =
                new Color(1.0f, 0.45f, 0.45f);
        }

        _progreso.Text =
            $"Pregunta {_posicion + 1} de {_preguntas.Count} · " +
            $"{_aciertos} aciertos";
        _botonSiguiente.Visible = true;
    }

    private void MostrarSiguientePregunta()
    {
        _posicion++;

        if (_posicion < _preguntas.Count)
        {
            MostrarPregunta();
            return;
        }

        _pantallaJuego.Visible = false;
        _pantallaFinal.Visible = true;
        _resultadoFinal.Text =
            $"Has conseguido {_aciertos} de " +
            $"{_preguntas.Count} aciertos.";
    }

    private void VolverAlInicio()
    {
        // Conservamos las categorías cargadas para no repetir la petición.
        _pantallaFinal.Visible = false;
        _pantallaJuego.Visible = false;
        _pantallaInicio.Visible = true;
        MostrarEstado(
            "Selecciona una categoría para comenzar otra partida.",
            false);
    }

    private async Task<T?> ObtenerJsonAsync<T>(string ruta)
    {
        string url = $"{ObtenerUrlApi()}/{ruta}";

        // Request comienza la petición y devuelve inmediatamente un Error.
        Error inicioPeticion = _http.Request(url);

        if (inicioPeticion != Error.Ok)
        {
            throw new InvalidOperationException(
                $"Godot no ha podido iniciar la petición: " +
                inicioPeticion);
        }

        // ToSignal espera sin bloquear la interfaz hasta recibir la respuesta.
        Variant[] respuesta = await ToSignal(
            _http,
            HttpRequest.SignalName.RequestCompleted);

        // El primer dato no es un código HTTP. Indica si Godot pudo completar
        // la comunicación: resolver el nombre, abrir la conexión y recibir
        // una respuesta. El valor Success equivale a una comunicación válida.
        long resultadoPeticion = respuesta[0].AsInt64();

        if (resultadoPeticion != (long)HttpRequest.Result.Success)
        {
            throw new InvalidOperationException(
                "La comunicación no se ha completado. Resultado de Godot: " +
                $"{(HttpRequest.Result)resultadoPeticion}.");
        }

        // El segundo dato sí es el código enviado por el servidor HTTP.
        long codigoHttp = respuesta[1].AsInt64();

        // El cuarto dato es el cuerpo de la respuesta en forma de bytes.
        byte[] cuerpo = respuesta[3].AsByteArray();
        string json = Encoding.UTF8.GetString(cuerpo);

        if (codigoHttp < 200 || codigoHttp >= 300)
        {
            throw new InvalidOperationException(
                $"El servidor ha respondido con el código " +
                $"{codigoHttp}.");
        }

        try
        {
            return JsonSerializer.Deserialize<T>(
                json,
                _opcionesJson);
        }
        catch (JsonException error)
        {
            // Este mensaje ayuda a distinguir un fallo de conexión de una
            // respuesta que no tiene la estructura JSON esperada.
            throw new InvalidOperationException(
                "El servidor ha respondido, pero el JSON no tiene el " +
                "formato esperado por el cliente.",
                error);
        }
    }

    private string ObtenerUrlApi()
    {
        string url = _entradaUrl.Text.Trim().TrimEnd('/');

        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException(
                "Escribe la dirección del servidor.");
        }

        // El usuario puede escribir la raíz o directamente la ruta /api.
        return url.EndsWith(
            "/api",
            StringComparison.OrdinalIgnoreCase)
            ? url
            : $"{url}/api";
    }

    private void MostrarEstado(string texto, bool esError)
    {
        _estado.Text = texto;
        _estado.Modulate = esError
            ? new Color(1.0f, 0.45f, 0.45f)
            : new Color(0.45f, 0.85f, 1.0f);
    }

    private static Label CrearEtiqueta(
        string texto,
        int tamano,
        HorizontalAlignment alineacion =
            HorizontalAlignment.Left)
    {
        Label etiqueta = new()
        {
            Text = texto,
            HorizontalAlignment = alineacion,
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        etiqueta.AddThemeFontSizeOverride(
            "font_size",
            tamano);

        return etiqueta;
    }

    private static Button CrearBoton(string texto)
    {
        Button boton = new()
        {
            Text = texto,
            CustomMinimumSize = new Vector2(0, 52),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        boton.AddThemeFontSizeOverride("font_size", 18);

        return boton;
    }

    private static StyleBoxFlat CrearEstiloTarjeta()
    {
        return new StyleBoxFlat
        {
            BgColor = new Color(0.095f, 0.13f, 0.21f),
            BorderColor = new Color(0.2f, 0.4f, 0.7f),
            BorderWidthLeft = 2,
            BorderWidthTop = 2,
            BorderWidthRight = 2,
            BorderWidthBottom = 2,
            CornerRadiusTopLeft = 16,
            CornerRadiusTopRight = 16,
            CornerRadiusBottomLeft = 16,
            CornerRadiusBottomRight = 16
        };
    }
}

// Los DTO reproducen la estructura del JSON devuelto por la API.
// No son entidades de Entity Framework y no acceden a SQLite.
public sealed class CategoriaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
}

public sealed class PreguntaDto
{
    public int Id { get; set; }
    public string Enunciado { get; set; } = "";
    public string[] Respuestas { get; set; } = [];
    public int RespuestaCorrecta { get; set; }
    public CategoriaDto Categoria { get; set; } = new();
}
