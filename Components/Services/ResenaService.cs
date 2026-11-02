using blazornacho.Data;
using blazornacho.Models;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace blazornacho.Services;

public class ResenaService
{
    private readonly IDbContextFactory<
        ConcesionariaDbContext> _factory;


    public ResenaService(
        IDbContextFactory<
            ConcesionariaDbContext> factory)
    {
        _factory = factory;
    }


    public List<Resena> ObtenerTodas()
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Resenas
            .AsNoTracking()
            .OrderByDescending(x => x.Fecha)
            .ToList();
    }


    public List<Resena> ObtenerPorVehiculo(
        string vehiculoId)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        return db.Resenas
            .AsNoTracking()
            .Where(
                x =>
                    x.VehiculoId ==
                    vehiculoId
            )
            .OrderByDescending(x => x.Fecha)
            .ToList();
    }


    public void Agregar(Resena resena)
    {
       
        Validator.ValidateObject(
         resena,
         new ValidationContext(resena),
         validateAllProperties: true
        );
        
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        resena.Fecha = DateTime.Now;

        db.Resenas.Add(resena);

        db.SaveChanges();
    }


    public void Eliminar(
        Resena resena)
    {
        using ConcesionariaDbContext db =
            _factory.CreateDbContext();

        Resena? encontrada =
            db.Resenas.Find(resena.Id);

        if (encontrada is null)
        {
            return;
        }

        db.Resenas.Remove(encontrada);

        db.SaveChanges();
    }
}