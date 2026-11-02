using blazornacho.Data;
using blazornacho.Models;

using Microsoft.EntityFrameworkCore;

namespace blazornacho.Services;

public class TurnoService
{
    private readonly IDbContextFactory<
        ConcesionariaDbContext> _factory;


    public TurnoService(
        IDbContextFactory<
            ConcesionariaDbContext> factory)
    {
        _factory = factory;
    }


    // ======================================================
    // PAGINACIÓN + BÚSQUEDA
    // ======================================================

    public PagedResult<Turno>
        ObtenerPaginado(
            int pagina,
            int tamanioPagina,
            string? busqueda,
            long usuarioId)
    {
        pagina = Math.Max(
            1,
            pagina
        );


        tamanioPagina =
            Math.Clamp(
                tamanioPagina,
                1,
                50
            );


        using ConcesionariaDbContext db =
            _factory.CreateDbContext();


        IQueryable<Turno> query =
            db.Turnos
                .AsNoTracking()

                .Where(
                    x =>
                        x.UsuarioId ==
                        usuarioId
                );


        // ==================================================
        // BÚSQUEDA REAL CONTRA LA BD
        // ==================================================

        
    if (!string.IsNullOrWhiteSpace(busqueda))
    {
    string texto = busqueda.Trim();

    // Permite encontrar coincidencias parciales.
    // Escapamos caracteres especiales de LIKE.
    string textoEscapado = texto
        .Replace("\\", "\\\\")
        .Replace("%", "\\%")
        .Replace("_", "\\_");

    string patron = $"%{textoEscapado}%";

    query = query.Where(x =>
        EF.Functions.Like(x.Nombre, patron, "\\") ||
        EF.Functions.Like(x.Motivo, patron, "\\") ||
        EF.Functions.Like(x.Hora, patron, "\\")
    );
    }



        int totalRegistros =
            query.Count();


        int totalPaginas =
            Math.Max(
                1,

                (int)Math.Ceiling(
                    totalRegistros /
                    (double)tamanioPagina
                )
            );


        pagina =
            Math.Min(
                pagina,
                totalPaginas
            );


        // ==================================================
        // PAGINACIÓN REAL CONTRA SQLITE
        // ==================================================

        List<Turno> items =
            query

                .OrderBy(x => x.Fecha)

                .ThenBy(x => x.Hora)

                .Skip(
                    (pagina - 1)
                    * tamanioPagina
                )

                .Take(tamanioPagina)

                .ToList();


        return new PagedResult<Turno>
        {
            Items = items,

            PaginaActual = pagina,

            TamanioPagina =
                tamanioPagina,

            TotalRegistros =
                totalRegistros,

            TotalPaginas =
                totalPaginas
        };
    }


    // ======================================================
    // CREAR
    // ======================================================

    public void Agregar(
    Turno turno)
   {
    using ConcesionariaDbContext db =
        _factory.CreateDbContext();


    // ======================================================
    // VERIFICAR QUE EL USUARIO SIGA EXISTIENDO
    // ======================================================

    bool usuarioExiste =
        db.Usuarios
            .AsNoTracking()
            .Any(
                x =>
                    x.Id ==
                    turno.UsuarioId
            );


    if (!usuarioExiste)
    {
        throw new InvalidOperationException(
            "La sesión actual pertenece a un usuario que ya no existe en la nueva base de datos. Cerrá sesión y volvé a registrarte o iniciar sesión."
        );
    }


    // ======================================================
    // DATOS DEL TURNO
    // ======================================================

    turno.Fecha =
        turno.Fecha.Date;


    turno.FechaReserva =
        DateTime.Now;


    // ======================================================
    // GUARDAR
    // ======================================================

    try
    {
        db.Turnos.Add(turno);

        db.SaveChanges();
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(
            "Error EF Core al guardar turno:"
        );

        Console.WriteLine(ex);


        Console.WriteLine(
            "InnerException:"
        );

        Console.WriteLine(
            ex.InnerException?.Message
        );


        throw new InvalidOperationException(
            "No se pudo guardar el turno en la base de datos. Verificá que la fecha y horario no estén ocupados.",
            ex
        );
    }
}


    // ======================================================
    // ACTUALIZAR
    // ======================================================

    public void Actualizar(
    Turno turno)
{
    using ConcesionariaDbContext db =
        _factory.CreateDbContext();


    bool usuarioExiste =
        db.Usuarios
            .AsNoTracking()
            .Any(
                x =>
                    x.Id ==
                    turno.UsuarioId
            );


    if (!usuarioExiste)
    {
        throw new InvalidOperationException(
            "La cuenta asociada a esta sesión ya no existe. Cerrá sesión y volvé a iniciar sesión."
        );
    }


    Turno? existente =
        db.Turnos
            .FirstOrDefault(
                x =>
                    x.Id ==
                        turno.Id
                    &&
                    x.UsuarioId ==
                        turno.UsuarioId
            );


    if (existente is null)
    {
        throw new InvalidOperationException(
            "No se encontró el turno que desea modificar."
        );
    }


    existente.Fecha =
        turno.Fecha.Date;


    existente.Hora =
        turno.Hora;


    existente.Nombre =
        turno.Nombre;


    existente.Motivo =
        turno.Motivo;


    try
    {
        db.SaveChanges();
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine(
            $"Error EF Core actualizando turno: {ex}"
        );


        Console.WriteLine(
            $"Detalle SQLite: {ex.InnerException?.Message}"
        );


        throw new InvalidOperationException(
            "No se pudo actualizar el turno. Verificá que la fecha y horario seleccionados estén disponibles.",
            ex
        );
    }
}


    // ======================================================
    // ELIMINAR
    // ======================================================

    public void Eliminar(
        long id,
        long usuarioId)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();


        Turno? turno =
            db.Turnos.FirstOrDefault(
                x =>
                    x.Id == id &&
                    x.UsuarioId ==
                        usuarioId
            );


        if (turno is null)
        {
            return;
        }


        db.Turnos.Remove(turno);

        db.SaveChanges();
    }


    // ======================================================
    // VALIDAR TURNO DUPLICADO
    // ======================================================

    public bool ExisteTurno(
        DateTime fecha,
        string hora,
        long? excluirId = null)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();


        IQueryable<Turno> query =
            db.Turnos
                .AsNoTracking()

                .Where(
                    x =>
                        x.Fecha ==
                            fecha.Date
                        &&
                        x.Hora ==
                            hora
                );


        if (excluirId.HasValue)
        {
            query =
                query.Where(
                    x =>
                        x.Id !=
                        excluirId.Value
                );
        }


        return query.Any();
    }
}