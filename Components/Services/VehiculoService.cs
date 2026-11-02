using blazornacho.Data;
using blazornacho.Models;

using Microsoft.EntityFrameworkCore;

namespace blazornacho.Services;

public class VehiculoService
{
    private readonly IDbContextFactory<
        ConcesionariaDbContext> _factory;


    public VehiculoService(
        IDbContextFactory<
            ConcesionariaDbContext> factory)
    {
        _factory = factory;
    }


    public List<Vehiculo> ObtenerTodos()
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Vehiculos
            .AsNoTracking()
            .OrderBy(x => x.Marca)
            .ThenBy(x => x.Modelo)
            .ToList();
    }


    public Vehiculo? ObtenerPorId(
        string id)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Vehiculos
            .AsNoTracking()
            .FirstOrDefault(
                x => x.Id == id
            );
    }


    public Vehiculo? ObtenerMasBarato()
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Vehiculos
            .AsNoTracking()
            .OrderBy(x => x.Precio)
            .FirstOrDefault();
    }


    public Vehiculo? ObtenerMasCaro()
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Vehiculos
            .AsNoTracking()
            .OrderByDescending(
                x => x.Precio
            )
            .FirstOrDefault();
    }
}