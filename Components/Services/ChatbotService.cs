using System.Globalization;
using System.Runtime.CompilerServices;
using blazornacho.Models;

namespace blazornacho.Services;

public class ChatbotService
{
    private readonly List<ChatMessage> _mensajes = new();

    private readonly LocalAiService _localAiService;
    private readonly SiteContextService _siteContextService;
    private readonly VehiculoService _vehiculoService;

    private const string MensajeFueraDeAmbito =
        "Solo puedo ayudarte con información relacionada con " +
        "Premium Quality Concesionaria. Puedo ayudarte con " +
        "vehículos, precios, características, compras, seguros, " +
        "reclamos, reseñas, turnos, contacto y con el funcionamiento " +
        "de esta página web.";

    
public ChatbotService(
    LocalAiService localAiService,
    SiteContextService siteContextService,
    VehiculoService vehiculoService,
    IConfiguration configuration)
{
    _localAiService = localAiService;
    _siteContextService = siteContextService;
    _vehiculoService = vehiculoService;

    _iaHabilitada =
        configuration.GetValue<bool>(
            "LocalAI:Enabled",
            true
        );

    ReiniciarConversacion();
}


    public List<ChatMessage> ObtenerMensajes()
    {
        return _mensajes.ToList();
    }

    public void AgregarMensajeUsuario(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return;
        }

        _mensajes.Add(new ChatMessage
        {
            Tipo = "user",
            Texto = texto.Trim(),
            Fecha = DateTime.Now
        });
    }



    private readonly bool _iaHabilitada;

    public ChatMessage CrearMensajeBotVacio()
    {
        ChatMessage mensaje = new()
        {
            Tipo = "bot",
            Texto = "",
            Fecha = DateTime.Now
        };

        _mensajes.Add(mensaje);

        return mensaje;
    }

    public async IAsyncEnumerable<string> ResponderStreamAsync(
        string pregunta,
        string rutaActual,
        [EnumeratorCancellation]
        CancellationToken cancellationToken = default)
    {
        string texto = Normalizar(pregunta);

        // =====================================================
        // 1. RESPUESTAS QUE PODEMOS SACAR DIRECTAMENTE
        //    DE LOS DATOS REALES DEL PROGRAMA
        // =====================================================

        string? respuestaLocal =
            IntentarResponderConDatosLocales(texto);

        if (!string.IsNullOrWhiteSpace(respuestaLocal))
        {
            yield return respuestaLocal;
            yield break;
        }

        // =====================================================
        // 2. BLOQUEAR SOLAMENTE TEMAS CLARAMENTE AJENOS
        // =====================================================

        if (EsTemaClaramenteExterno(texto))
        {
            yield return MensajeFueraDeAmbito;
            yield break;
        }

        
        if (!_iaHabilitada)
        {
        yield return
        "La IA avanzada no está disponible en " +
        "esta versión pública. Puedo ayudarte " +
        "con los vehículos disponibles, precios " +
        "y características, o podés consultar " +
        "las secciones Modelos, Seguros, " +
        "Reseñas, Turnos y Contacto.";

        yield break;
        }


        // =====================================================
        // 3. NO BLOQUEAMOS EL RESTO
        //
        // Si existe alguna posibilidad de que la pregunta
        // esté relacionada con la concesionaria,
        // pasa a la IA.
        // =====================================================

        List<ChatMessage> historial =
            _mensajes
                .Where(m =>
                    !string.IsNullOrWhiteSpace(m.Texto))
                .TakeLast(4)
                .ToList();

        string contexto =
            await _siteContextService
                .ConstruirContextoAsync(
                    cancellationToken
                );

        // =====================================================
        // 4. UNA SOLA LLAMADA A OLLAMA
        // =====================================================

        await foreach (
            string fragmento in
            _localAiService
                .GenerarRespuestaStreamAsync(
                    contexto,
                    historial,
                    rutaActual,
                    cancellationToken
                )
        )
        {
            yield return fragmento;
        }
    }

    // =========================================================
    // RESPUESTAS DIRECTAS DESDE LOS DATOS DEL PROGRAMA
    // =========================================================

    private string? IntentarResponderConDatosLocales(
        string texto)
    {
        // -----------------------------------------------------
        // SALUDO
        // -----------------------------------------------------

        if (EsSaludo(texto))
        {
            return
                "¡Hola! Puedo ayudarte con vehículos, precios, " +
                "características, compras, seguros, reclamos, " +
                "reseñas, turnos y demás información de " +
                "Premium Quality.";
        }

        // -----------------------------------------------------
        // VEHÍCULO MÁS BARATO
        // -----------------------------------------------------

        if (EsConsultaVehiculoMasBarato(texto))
        {
            Vehiculo? auto =
                _vehiculoService.ObtenerMasBarato();

            if (auto == null)
            {
                return
                    "Actualmente no hay vehículos disponibles.";
            }

            return
                $"El vehículo con el precio más bajo actualmente " +
                $"es el {auto.Marca} {auto.Modelo}, con un precio " +
                $"de {FormatearPrecio(auto.Precio)}. " +
                $"Podés encontrarlo en la sección Modelos.";
        }

        // -----------------------------------------------------
        // VEHÍCULO MÁS CARO
        // -----------------------------------------------------

        if (EsConsultaVehiculoMasCaro(texto))
        {
            Vehiculo? auto =
                _vehiculoService.ObtenerMasCaro();

            if (auto == null)
            {
                return
                    "Actualmente no hay vehículos disponibles.";
            }

            return
                $"El vehículo con el precio más alto actualmente " +
                $"es el {auto.Marca} {auto.Modelo}, con un precio " +
                $"de {FormatearPrecio(auto.Precio)}. " +
                $"Podés encontrarlo en la sección Modelos.";
        }

        // -----------------------------------------------------
        // LISTADO DE VEHÍCULOS
        // -----------------------------------------------------

        if (EsConsultaListadoVehiculos(texto))
        {
            return ConstruirListadoVehiculos();
        }

        return null;
    }

    // =========================================================
    // DETECTAR VEHÍCULO MÁS BARATO
    // =========================================================

    private static bool EsConsultaVehiculoMasBarato(
        string texto)
    {
        return
            texto.Contains("precio mas bajo") ||
            texto.Contains("menor precio") ||
            texto.Contains("mas barato") ||
            texto.Contains("mas barata") ||
            texto.Contains("mas economico") ||
            texto.Contains("mas economica") ||
            texto.Contains("cuesta menos") ||
            texto.Contains("precio menor");
    }

    // =========================================================
    // DETECTAR VEHÍCULO MÁS CARO
    // =========================================================

    private static bool EsConsultaVehiculoMasCaro(
        string texto)
    {
        return
            texto.Contains("precio mas alto") ||
            texto.Contains("mayor precio") ||
            texto.Contains("mas caro") ||
            texto.Contains("mas cara") ||
            texto.Contains("cuesta mas") ||
            texto.Contains("precio mayor");
    }

    // =========================================================
    // LISTADO DE VEHÍCULOS
    // =========================================================

    private static bool EsConsultaListadoVehiculos(
        string texto)
    {
        bool hablaDeVehiculos =
            texto.Contains("auto") ||
            texto.Contains("autos") ||
            texto.Contains("vehiculo") ||
            texto.Contains("vehiculos") ||
            texto.Contains("modelo") ||
            texto.Contains("modelos");

        bool quiereListado =
            texto.Contains("disponible") ||
            texto.Contains("disponibles") ||
            texto.Contains("que autos") ||
            texto.Contains("que vehiculos") ||
            texto.Contains("que modelos") ||
            texto.Contains("todos los autos") ||
            texto.Contains("todos los vehiculos") ||
            texto.Contains("lista");

        return
            hablaDeVehiculos &&
            quiereListado;
    }

    private string ConstruirListadoVehiculos()
    {
        List<Vehiculo> vehiculos =
            _vehiculoService.ObtenerTodos();

        if (vehiculos.Count == 0)
        {
            return
                "Actualmente no hay vehículos disponibles.";
        }

        string listado =
            string.Join(
                "\n",
                vehiculos.Select(
                    (auto, indice) =>
                        $"{indice + 1}. " +
                        $"{auto.Marca} " +
                        $"{auto.Modelo}"
                )
            );

        return
            $"Actualmente tenemos {vehiculos.Count} vehículos disponibles:\n" +
            listado +
            "\nPodés verlos en la sección Modelos.";
    }

    // =========================================================
    // TEMAS EXTERNOS
    // =========================================================

    private static bool EsTemaClaramenteExterno(
        string texto)
    {
        // Si el usuario está hablando explícitamente
        // sobre alguna función de la concesionaria,
        // NO bloqueamos la consulta aunque aparezca
        // alguna palabra externa.
        string[] referenciasConcesionaria =
        {
            "concesionaria",
            "premium quality",
            "auto",
            "autos",
            "vehiculo",
            "vehiculos",
            "modelo",
            "marca",
            "precio",
            "motor",
            "potencia",
            "transmision",
            "traccion",
            "seguro",
            "seguros",
            "reclamo",
            "reclamos",
            "siniestro",
            "turno",
            "turnos",
            "resena",
            "resenas",
            "contacto",
            "comprar",
            "compra",
            "pago",
            "pagina",
            "sitio"
        };

        bool pareceRelacionado =
            referenciasConcesionaria
                .Any(texto.Contains);

        if (pareceRelacionado)
        {
            return false;
        }

        // Consultas claramente ajenas.
        string[] temasExternos =
        {
            "futbol",
            "messi",
            "boca",
            "river",
            "mundial",
            "deporte",
            "basquet",
            "tenis",
            "pizza",
            "receta",
            "cocina",
            "comida",
            "politica",
            "presidente",
            "elecciones",
            "minecraft",
            "fortnite",
            "videojuego",
            "videojuegos",
            "pelicula",
            "peliculas",
            "serie",
            "series",
            "celebridad",
            "medicina",
            "enfermedad"
        };

        return
            temasExternos.Any(
                texto.Contains
            );
    }

    // =========================================================
    // SALUDOS
    // =========================================================

    private static bool EsSaludo(
        string texto)
    {
        return
            texto == "hola" ||
            texto == "buenas" ||
            texto == "buen dia" ||
            texto == "buenas tardes" ||
            texto == "buenas noches" ||
            texto == "hey";
    }

    // =========================================================
    // PRECIO
    // =========================================================

    private static string FormatearPrecio(
        decimal precio)
    {
        CultureInfo cultura =
            CultureInfo.GetCultureInfo(
                "es-AR"
            );

        return
            $"${precio.ToString("N0", cultura)} ARS";
    }

    // =========================================================
    // NORMALIZAR
    // =========================================================

    private static string Normalizar(
        string texto)
    {
        return texto
            .Trim()
            .ToLowerInvariant()
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ú", "u")
            .Replace("ü", "u")
            .Replace("ñ", "n");
    }

    // =========================================================
    // REINICIAR CHAT
    // =========================================================

    public void ReiniciarConversacion()
    {
        _mensajes.Clear();

        _mensajes.Add(new ChatMessage
        {
            Tipo = "bot",

            Texto =
                "¡Bienvenido a Premium Quality! " +
                "Soy el asistente inteligente de la " +
                "concesionaria. ¿En qué puedo ayudarte?",

            Fecha = DateTime.Now
        });
    }
}