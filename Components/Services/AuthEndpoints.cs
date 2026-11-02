using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using blazornacho.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace blazornacho.Services;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group =
            endpoints.MapGroup("/auth");

        group.MapPost(
            "/login",
            IniciarSesionAsync
        );

        group.MapPost(
            "/registro",
            RegistrarAsync
        );

        group.MapPost(
            "/logout",
            CerrarSesionAsync
        );

        group.MapPost(
            "/recuperar-password",
            RecuperarPasswordAsync
        );


        return endpoints;
    }

    private static async Task<IResult>
        IniciarSesionAsync(
            HttpContext context,
            UsuarioService usuarioService,
            IAntiforgery antiforgery)
    {
        await antiforgery
            .ValidateRequestAsync(context);

        IFormCollection form =
            await context.Request
                .ReadFormAsync();

        LoginUsuarioModel model = new()
        {
            Correo =
                form["Correo"].ToString(),

            Password =
                form["Password"].ToString()
        };

        string returnUrl =
            NormalizarReturnUrl(
                form["ReturnUrl"].ToString()
            );

        if (!EsModeloValido(
                model,
                out string error))
        {
            return RedirectConMensaje(
                "/iniciar-sesion",
                "error",
                error,
                returnUrl
            );
        }

        Usuario? usuario =
            usuarioService.ValidarCredenciales(
                model.Correo,
                model.Password
            );

        if (usuario is null)
        {
            return RedirectConMensaje(
                "/iniciar-sesion",
                "error",
                "El correo o la contraseña son incorrectos.",
                returnUrl
            );
        }

        await IniciarCookieAsync(
            context,
            usuario
        );

        return Results.Redirect(
            returnUrl
        );
    }

    private static async Task<IResult>
    RegistrarAsync(
        HttpContext context,
        UsuarioService usuarioService,
        IAntiforgery antiforgery)
{
    try
    {
        await antiforgery
            .ValidateRequestAsync(context);

        IFormCollection form =
            await context.Request
                .ReadFormAsync();

        RegistroUsuarioModel model = new()
        {
            Nombre =
                form["Nombre"].ToString(),

            Apellido =
                form["Apellido"].ToString(),

            Correo =
                form["Correo"].ToString(),

            Password =
                form["Password"].ToString(),

            ConfirmarPassword =
                form["ConfirmarPassword"]
                    .ToString(),

            CodigoRecuperacion =
                form["CodigoRecuperacion"]
                    .ToString()
        };

        // ==========================================
        // VALIDAR DATOS
        // ==========================================

        if (!EsModeloValido(
                model,
                out string error))
        {
            return RedirectConMensaje(
                "/registrarse",
                "error",
                error
            );
        }


        // ==========================================
        // COMPROBAR CORREO REPETIDO
        // ==========================================

        if (usuarioService
            .CorreoExiste(model.Correo))
        {
            return RedirectConMensaje(
                "/registrarse",
                "error",
                "Ya existe una cuenta registrada con ese correo electrónico."
            );
        }


        // ==========================================
        // REGISTRAR USUARIO
        // ==========================================

        Usuario usuario =
            usuarioService.Registrar(
                model.Nombre,
                model.Apellido,
                model.Correo,
                model.Password,
                model.CodigoRecuperacion
            );


        // ==========================================
        // INICIAR SESIÓN AUTOMÁTICAMENTE
        // ==========================================

        await IniciarCookieAsync(
            context,
            usuario
        );


        // Registro correcto.
        return Results.Redirect(
            "/mi-cuenta?registro=ok"
        );
    }

    catch (AntiforgeryValidationException)
    {
        return RedirectConMensaje(
            "/registrarse",
            "error",
            "La sesión del formulario venció. Volvé a completar el registro."
        );
    }

    catch (InvalidOperationException ex)
    {
        return RedirectConMensaje(
            "/registrarse",
            "error",
            ex.Message
        );
    }

    catch (Exception ex)
    {
        Console.WriteLine(
            $"Error registrando usuario: {ex}"
        );

        return RedirectConMensaje(
            "/registrarse",
            "error",
            "No se pudo registrar el usuario. Revisá los datos ingresados e intentá nuevamente."
        );
    }
}

    private static async Task<IResult>
        CerrarSesionAsync(
            HttpContext context,
            IAntiforgery antiforgery)
    {
        await antiforgery
            .ValidateRequestAsync(context);

        await context.SignOutAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme
        );

        return Results.Redirect("/");
    }

    private static async Task<IResult>
        RecuperarPasswordAsync(
            HttpContext context,
            UsuarioService usuarioService,
            IAntiforgery antiforgery)
    {
        await antiforgery
            .ValidateRequestAsync(context);

        IFormCollection form =
            await context.Request
                .ReadFormAsync();

        RecuperarPasswordModel model = new()
        {
            Correo =
                form["Correo"].ToString(),

            CodigoRecuperacion =
                form["CodigoRecuperacion"]
                    .ToString(),

            NuevaPassword =
                form["NuevaPassword"]
                    .ToString(),

            ConfirmarPassword =
                form["ConfirmarPassword"]
                    .ToString()
        };

        if (!EsModeloValido(
                model,
                out string error))
        {
            return RedirectConMensaje(
                "/recuperar-password",
                "error",
                error
            );
        }

        bool actualizado =
            usuarioService.RestablecerPassword(
                model.Correo,
                model.CodigoRecuperacion,
                model.NuevaPassword
            );

        if (!actualizado)
        {
            return RedirectConMensaje(
                "/recuperar-password",
                "error",
                "No se pudo verificar el correo y el PIN de recuperación."
            );
        }

        return RedirectConMensaje(
            "/iniciar-sesion",
            "mensaje",
            "Contraseña actualizada. Ya podés iniciar sesión con la nueva contraseña."
        );
    }

    // ======================================================
    // CREAR COOKIE DE SESIÓN
    // ======================================================

    private static async Task IniciarCookieAsync(
        HttpContext context,
        Usuario usuario)
    {
        Claim[] claims =
        {
            new(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()
            ),

            new(
                ClaimTypes.Name,
                usuario.NombreCompleto
            ),

            new(
                ClaimTypes.Email,
                usuario.Correo
            )
        };

        ClaimsIdentity identity = new(
            claims,
            CookieAuthenticationDefaults
                .AuthenticationScheme
        );

        ClaimsPrincipal principal =
            new(identity);

        AuthenticationProperties properties =
        new()
        {
        // Cookie solamente de sesión.
        // No se guarda como cookie persistente.
        IsPersistent = false,

        AllowRefresh = true
        };

        await context.SignInAsync(
            CookieAuthenticationDefaults
                .AuthenticationScheme,

            principal,
            properties
        );
    }

    private static bool EsModeloValido(
        object model,
        out string error)
    {
        ValidationContext context =
            new(model);

        List<ValidationResult> results =
            new();

        bool valido =
            Validator.TryValidateObject(
                model,
                context,
                results,
                validateAllProperties: true
            );

        error = valido
            ? string.Empty
            : results
                    .FirstOrDefault()
                    ?.ErrorMessage
                ?? "Revisá los datos ingresados.";

        return valido;
    }

    private static IResult RedirectConMensaje(
        string ruta,
        string clave,
        string mensaje,
        string? returnUrl = null)
    {
        string destino =
            $"{ruta}?{clave}=" +
            Uri.EscapeDataString(mensaje);

        if (!string.IsNullOrWhiteSpace(
                returnUrl) &&
            returnUrl != "/")
        {
            destino +=
                $"&returnUrl=" +
                Uri.EscapeDataString(
                    returnUrl
                );
        }

        return Results.Redirect(destino);
    }

    private static string NormalizarReturnUrl(
        string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(
                returnUrl))
        {
            return "/";
        }

        string value =
            returnUrl.Trim();

        if (!value.StartsWith('/') ||
            value.StartsWith("//") ||
            value.Contains(
                "://",
                StringComparison.Ordinal
            ))
        {
            return "/";
        }

        return value;
    }
}