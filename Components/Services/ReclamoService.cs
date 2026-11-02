using blazornacho.Data;
using blazornacho.Models;

using Microsoft.EntityFrameworkCore;

namespace blazornacho.Services;

public class ReclamoService
{
    private readonly IDbContextFactory<
        ConcesionariaDbContext> _factory;


    public ReclamoService(
        IDbContextFactory<
            ConcesionariaDbContext> factory)
    {
        _factory = factory;
    }


    public List<Reclamo> ObtenerTodos(
        long usuarioId)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Reclamos
            .AsNoTracking()

            .Where(
                x =>
                    x.UsuarioId ==
                    usuarioId
            )

            .OrderByDescending(
                x => x.FechaEnvio
            )

            .ToList();
    }


    public void Agregar(
        Reclamo reclamo)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        reclamo.FechaEnvio =
            DateTime.Now;

        db.Reclamos.Add(reclamo);

        db.SaveChanges();
    }


    public void Eliminar(
        long id,
        long usuarioId)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        Reclamo? reclamo =
            db.Reclamos.FirstOrDefault(
                x =>
                    x.Id == id &&
                    x.UsuarioId == usuarioId
            );


        if (reclamo is null)
        {
            return;
        }


        db.Reclamos.Remove(reclamo);

        db.SaveChanges();
    }
}