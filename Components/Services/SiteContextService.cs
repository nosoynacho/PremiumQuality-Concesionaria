using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace blazornacho.Services;

public class SiteContextService
{
    private readonly IWebHostEnvironment _environment;

    private readonly VehiculoService _vehiculoService;

    private readonly ResenaService _resenaService;

    public SiteContextService(
        IWebHostEnvironment environment,
        VehiculoService vehiculoService,
        ResenaService resenaService)
    {
        _environment = environment;

        _vehiculoService =
            vehiculoService;

        _resenaService =
            resenaService;
    }

    public async Task<string> ConstruirContextoAsync(
        CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        AgregarVehiculos(sb);

        AgregarResenasResumidas(sb);

        AgregarRutasRegistradas(sb);

        await AgregarContenidoActualDePaginasAsync(
            sb,
            cancellationToken
        );

        return sb.ToString();
    }

    // =========================================
    // VEHÍCULOS
    // =========================================

    private void AgregarVehiculos(
        StringBuilder sb)
    {
        sb.AppendLine(
            "VEHÍCULOS DISPONIBLES:"
        );

        foreach (
            var auto in
            _vehiculoService.ObtenerTodos()
        )
        {
            sb.AppendLine(
                $"""
                - {auto.Marca} {auto.Modelo}
                  ID: {auto.Id}
                  Año: {auto.Anio}
                  Segmento: {auto.Segmento}
                  Motor: {auto.Motor}
                  Potencia: {auto.Potencia}
                  Transmisión: {auto.Transmision}
                  Tracción: {auto.Traccion}
                  Asientos: {auto.Asientos}
                  Color: {auto.Color}
                  Precio: ${auto.Precio:N0} ARS
                """
            );
        }

        sb.AppendLine();
    }

    // =========================================
    // RESEÑAS
    // =========================================

    private void AgregarResenasResumidas(
        StringBuilder sb)
    {
        sb.AppendLine(
            "RESEÑAS DE VEHÍCULOS:"
        );

        foreach (
            var auto in
            _vehiculoService.ObtenerTodos()
        )
        {
            var resenas =
                _resenaService
                    .ObtenerPorVehiculo(auto.Id);

            if (resenas.Count == 0)
            {
                sb.AppendLine(
                    $"- {auto.Marca} {auto.Modelo}: " +
                    "sin reseñas."
                );

                continue;
            }

            double promedio =
                resenas.Average(
                    r => r.Puntaje
                );

            sb.AppendLine(
                $"- {auto.Marca} {auto.Modelo}: " +
                $"{resenas.Count} reseña(s), " +
                $"promedio {promedio:0.0}/5."
            );
        }

        sb.AppendLine();
    }

    // =========================================
    // RUTAS BLAZOR
    // =========================================

    private static void AgregarRutasRegistradas(
        StringBuilder sb)
    {
        sb.AppendLine(
            "RUTAS BLAZOR REGISTRADAS:"
        );

        var rutas =
            typeof(Program)
                .Assembly
                .GetTypes()
                .SelectMany(
                    tipo =>
                        tipo
                            .GetCustomAttributes(
                                typeof(RouteAttribute),
                                inherit: false
                            )
                            .Cast<RouteAttribute>()
                            .Select(
                                atributo =>
                                    new
                                    {
                                        Pagina =
                                            tipo.Name,

                                        Ruta =
                                            atributo.Template
                                    }
                            )
                )
                .OrderBy(x => x.Ruta)
                .ToList();

        foreach (var ruta in rutas)
        {
            sb.AppendLine(
                $"- {ruta.Pagina}: {ruta.Ruta}"
            );
        }

        sb.AppendLine();
    }

    // =========================================
    // LECTURA AUTOMÁTICA DE PÁGINAS
    // =========================================

    private async Task
        AgregarContenidoActualDePaginasAsync(
            StringBuilder sb,
            CancellationToken cancellationToken)
    {
        string pagesFolder =
            Path.Combine(
                _environment.ContentRootPath,
                "Components",
                "Pages"
            );

        if (!Directory.Exists(pagesFolder))
        {
            return;
        }

        sb.AppendLine(
            "CONTENIDO VISIBLE DE LAS PÁGINAS:"
        );

        foreach (
            string file in
            Directory.EnumerateFiles(
                pagesFolder,
                "*.razor"
            )
        )
        {
            string source =
                await File.ReadAllTextAsync(
                    file,
                    cancellationToken
                );

            string? route =
                ExtraerRuta(source);

            string visibleText =
                ExtraerTextoVisible(source);

            if (
                string.IsNullOrWhiteSpace(
                    visibleText
                )
            )
            {
                continue;
            }

            sb.AppendLine(
                $"- Página " +
                $"{Path.GetFileNameWithoutExtension(file)} " +
                $"({route ?? "sin ruta"}): " +
                visibleText
            );
        }

        sb.AppendLine();
    }

    // =========================================
    // EXTRAER @page
    // =========================================

    private static string? ExtraerRuta(
        string source)
    {
        Match match =
            Regex.Match(
                source,
                "@page\\s+\"([^\"]+)\"",
                RegexOptions.IgnoreCase
            );

        return match.Success
            ? match.Groups[1].Value
            : null;
    }

    // =========================================
    // EXTRAER TEXTO VISIBLE DE RAZOR
    // =========================================

    private static string ExtraerTextoVisible(
        string source)
    {
        int codeIndex =
            source.IndexOf(
                "@code",
                StringComparison.OrdinalIgnoreCase
            );

        // No necesitamos mandar todo el
        // código C# a la IA.
        if (codeIndex >= 0)
        {
            source =
                source[..codeIndex];
        }

        // Quitamos directivas Razor.
        source =
            Regex.Replace(
                source,
                "^\\s*@(page|rendermode|using|" +
                "inject|attribute|layout).*?$",
                " ",
                RegexOptions.Multiline |
                RegexOptions.IgnoreCase
            );

        var textos =
            new List<string>();

        // Texto que aparece entre etiquetas.
        foreach (
            Match match in
            Regex.Matches(
                source,
                ">(.*?)<",
                RegexOptions.Singleline
            )
        )
        {
            string text =
                LimpiarFragmento(
                    match.Groups[1].Value
                );

            if (
                !string.IsNullOrWhiteSpace(
                    text
                )
            )
            {
                textos.Add(text);
            }
        }

        // También lee placeholders,
        // títulos y aria-labels.
        foreach (
            Match match in
            Regex.Matches(
                source,
                "(?:placeholder|aria-label|title)=" +
                "\"([^\"]+)\"",
                RegexOptions.IgnoreCase
            )
        )
        {
            string text =
                LimpiarFragmento(
                    match.Groups[1].Value
                );

            if (
                !string.IsNullOrWhiteSpace(
                    text
                )
            )
            {
                textos.Add(text);
            }
        }

        return string.Join(
            " | ",
            textos.Distinct(
                StringComparer.OrdinalIgnoreCase
            )
        );
    }

    private static string LimpiarFragmento(
        string value)
    {
        string text =
            WebUtility.HtmlDecode(value);

        // Eliminamos expresiones Razor sencillas.
        text =
            Regex.Replace(
                text,
                "@[^\\s<]+",
                " "
            );

        text =
            Regex.Replace(
                text,
                "\\s+",
                " "
            ).Trim();

        if (
            text.Length < 2 ||
            text is "{" or "}"
        )
        {
            return "";
        }

        return text;
    }
}