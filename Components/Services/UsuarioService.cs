using blazornacho.Data;
using blazornacho.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace blazornacho.Services;

public class UsuarioService
{
    private readonly IDbContextFactory<
        ConcesionariaDbContext> _factory;


    private readonly PasswordHasher<Usuario>
        _passwordHasher = new();


    public UsuarioService(
        IDbContextFactory<
            ConcesionariaDbContext> factory)
    {
        _factory = factory;
    }


    // ======================================================
    // BUSCAR POR ID
    // ======================================================

    public Usuario? ObtenerPorId(long id)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Usuarios
            .AsNoTracking()
            .FirstOrDefault(
                x => x.Id == id
            );
    }


    // ======================================================
    // BUSCAR POR CORREO
    // ======================================================

    public Usuario? ObtenerPorCorreo(
        string correo)
    {
        string correoNormalizado =
            NormalizarCorreo(correo);

        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Usuarios
            .AsNoTracking()
            .FirstOrDefault(
                x =>
                    x.Correo ==
                    correoNormalizado
            );
    }


    // ======================================================
    // VERIFICAR SI EL CORREO EXISTE
    // ======================================================

    public bool CorreoExiste(
        string correo)
    {
        string correoNormalizado =
            NormalizarCorreo(correo);

        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Usuarios.Any(
            x =>
                x.Correo ==
                correoNormalizado
        );
    }


    // ======================================================
    // REGISTRAR
    // ======================================================

    public Usuario Registrar(
        string nombre,
        string apellido,
        string correo,
        string password,
        string codigoRecuperacion)
    {
        string correoNormalizado =
            NormalizarCorreo(correo);


        if (CorreoExiste(correoNormalizado))
        {
            throw new InvalidOperationException(
                "Ya existe una cuenta registrada con ese correo."
            );
        }


        Usuario usuario = new()
        {
            Nombre = nombre.Trim(),

            Apellido = apellido.Trim(),

            Correo = correoNormalizado,

            FechaRegistro = DateTime.Now,

            Activo = true
        };


        usuario.PasswordHash =
            _passwordHasher.HashPassword(
                usuario,
                password
            );


        usuario.CodigoRecuperacionHash =
            _passwordHasher.HashPassword(
                usuario,
                codigoRecuperacion
            );


        using ConcesionariaDbContext db =
            _factory.CreateDbContext();


        try
        {
            db.Usuarios.Add(usuario);

            db.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(
                "No se pudo registrar la cuenta. Verificá que el correo no esté registrado.",
                ex
            );
        }


        return usuario;
    }


    // ======================================================
    // LOGIN
    // ======================================================

    public Usuario? ValidarCredenciales(
        string correo,
        string password)
    {
        Usuario? usuario =
            ObtenerPorCorreo(correo);


        if (usuario is null ||
            !usuario.Activo)
        {
            return null;
        }


        PasswordVerificationResult result =
            _passwordHasher
                .VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    password
                );


        if (result ==
            PasswordVerificationResult.Failed)
        {
            return null;
        }


        return usuario;
    }


    // ======================================================
    // RECUPERAR CONTRASEÑA
    // ======================================================

    public bool RestablecerPassword(
        string correo,
        string codigoRecuperacion,
        string nuevaPassword)
    {
        string correoNormalizado =
            NormalizarCorreo(correo);


        using ConcesionariaDbContext db =
            _factory.CreateDbContext();


        Usuario? usuario =
            db.Usuarios.FirstOrDefault(
                x =>
                    x.Correo ==
                    correoNormalizado
            );


        if (usuario is null ||
            !usuario.Activo)
        {
            return false;
        }


        PasswordVerificationResult codigoValido =
            _passwordHasher
                .VerifyHashedPassword(
                    usuario,

                    usuario
                        .CodigoRecuperacionHash,

                    codigoRecuperacion
                );


        if (codigoValido ==
            PasswordVerificationResult.Failed)
        {
            return false;
        }


        usuario.PasswordHash =
            _passwordHasher.HashPassword(
                usuario,
                nuevaPassword
            );


        db.SaveChanges();


        return true;
    }


    private static string NormalizarCorreo(
        string correo)
    {
        return (correo ?? string.Empty)
            .Trim()
            .ToLowerInvariant();
    }
}