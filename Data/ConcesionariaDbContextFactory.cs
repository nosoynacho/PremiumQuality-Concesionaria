using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace blazornacho.Data;

public class ConcesionariaDbContextFactory
    : IDesignTimeDbContextFactory<
        ConcesionariaDbContext>
{
    public ConcesionariaDbContext
        CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<
            ConcesionariaDbContext> options =
                new();

        options.UseSqlite(
            "Data Source=Data/concesionaria.db"
        );

        return new ConcesionariaDbContext(
            options.Options
        );
    }
}