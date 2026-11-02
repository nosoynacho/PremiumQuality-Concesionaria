using blazornacho.Models;
using Microsoft.EntityFrameworkCore;

namespace blazornacho.Data;

public class ConcesionariaDbContext : DbContext
{
    public ConcesionariaDbContext(
        DbContextOptions<ConcesionariaDbContext> options)
        : base(options)
    {
    }


    // =========================================================
    // TABLAS
    // =========================================================

    public DbSet<Usuario> Usuarios =>
        Set<Usuario>();

    public DbSet<Vehiculo> Vehiculos =>
        Set<Vehiculo>();

    public DbSet<Resena> Resenas =>
        Set<Resena>();

    public DbSet<Turno> Turnos =>
        Set<Turno>();

    public DbSet<Reclamo> Reclamos =>
        Set<Reclamo>();


    // =========================================================
    // CONFIGURACIÓN DE TABLAS Y RELACIONES
    // =========================================================

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // =====================================================
        // USUARIOS
        // =====================================================

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");

            entity.HasKey(x => x.Id);


            entity.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(60);


            entity.Property(x => x.Apellido)
                .IsRequired()
                .HasMaxLength(60);


            entity.Property(x => x.Correo)
                .IsRequired()
                .HasMaxLength(150)
                .UseCollation("NOCASE");


            entity.Property(x => x.PasswordHash)
                .IsRequired();


            entity.Property(
                    x => x.CodigoRecuperacionHash)
                .IsRequired();


            entity.Property(x => x.FechaRegistro)
                .IsRequired();


            entity.Property(x => x.Activo)
                .IsRequired();


            // No puede haber dos usuarios
            // con el mismo correo.
            entity.HasIndex(x => x.Correo)
                .IsUnique();
        });


        // =====================================================
        // VEHÍCULOS
        // =====================================================

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.ToTable("Vehiculos");

            entity.HasKey(x => x.Id);


            entity.Property(x => x.Id)
                .HasMaxLength(40);


            entity.Property(x => x.Marca)
                .IsRequired()
                .HasMaxLength(60);


            entity.Property(x => x.Modelo)
                .IsRequired()
                .HasMaxLength(100);


            entity.Property(x => x.Anio)
                .IsRequired()
                .HasMaxLength(10);


            entity.Property(x => x.Motor)
                .HasMaxLength(100);


            entity.Property(x => x.Potencia)
                .HasMaxLength(60);


            entity.Property(x => x.Transmision)
                .HasMaxLength(100);


            entity.Property(x => x.Traccion)
                .HasMaxLength(60);


            entity.Property(x => x.Asientos)
                .HasMaxLength(20);


            entity.Property(x => x.Color)
                .HasMaxLength(60);


            entity.Property(x => x.Precio)
                .HasPrecision(18, 2);


            entity.Property(x => x.Segmento)
                .HasMaxLength(60);


            entity.Property(x => x.Imagen)
                .HasMaxLength(250);
        });


        // =====================================================
        // TURNOS
        // =====================================================

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.ToTable("Turnos");

            entity.HasKey(x => x.Id);


            entity.Property(x => x.Hora)
                .IsRequired()
                .HasMaxLength(5);


            entity.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(80);


            entity.Property(x => x.Motivo)
                .IsRequired()
                .HasMaxLength(250);


            // Nadie puede reservar exactamente
            // la misma fecha y hora.
            entity.HasIndex(
                x => new
                {
                    x.Fecha,
                    x.Hora
                }
            )
            .IsUnique();


            // Usuario 1 ----- N Turnos
            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.Turnos)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // =====================================================
        // RECLAMOS
        // =====================================================

        modelBuilder.Entity<Reclamo>(entity =>
        {
            entity.ToTable("Reclamos");

            entity.HasKey(x => x.Id);


            entity.Property(x => x.Nombre)
                .IsRequired()
                .HasMaxLength(100);


            entity.Property(x => x.Correo)
                .IsRequired()
                .HasMaxLength(150);


            entity.Property(x => x.Telefono)
                .HasMaxLength(30);


            entity.Property(x => x.Seguro)
                .HasMaxLength(100);


            entity.Property(x => x.Marca)
                .HasMaxLength(100);


            entity.Property(x => x.Modelo)
                .HasMaxLength(100);


            entity.Property(x => x.Patente)
                .HasMaxLength(20);


            entity.Property(x => x.Siniestro)
                .HasMaxLength(50);


            entity.Property(x => x.Mensaje)
                .HasMaxLength(1000);


            // Usuario 1 ----- N Reclamos
            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.Reclamos)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // =====================================================
        // RESEÑAS
        // =====================================================

        modelBuilder.Entity<Resena>(entity =>
        {
            entity.ToTable("Resenas");

            entity.HasKey(x => x.Id);


            entity.Property(x => x.Comentario)
                .IsRequired()
                .HasMaxLength(1000);


            // Vehículo 1 ----- N Reseñas
            entity.HasOne(x => x.Vehiculo)
                .WithMany(x => x.Resenas)
                .HasForeignKey(x => x.VehiculoId)
                .OnDelete(DeleteBehavior.Cascade);


            // Usuario 1 ----- N Reseñas
            entity.HasOne(x => x.Usuario)
                .WithMany(x => x.Resenas)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        // =====================================================
        // DATOS INICIALES DE VEHÍCULOS
        // =====================================================

        modelBuilder.Entity<Vehiculo>().HasData(

            new Vehiculo
            {
                Id = "f150",
                Marca = "Ford",
                Modelo = "F-150 Trenor",
                Anio = "2025",
                Motor = "5.0L Coyote V8",
                Potencia = "406 CV",
                Transmision =
                    "Automática 10 velocidades",
                Traccion = "4x4",
                Asientos = "5",
                Color = "Negro Ebony",
                Precio = 135375000m,
                Segmento = "Pick-up",
                Imagen = "img/ford.jpg"
            },


            new Vehiculo
            {
                Id = "golfgti",
                Marca = "Volkswagen",
                Modelo = "Golf GTI",
                Anio = "2023",
                Motor = "2.0 TSI turbo",
                Potencia = "241 hp",
                Transmision =
                    "DSG 7 velocidades",
                Traccion = "Delantera",
                Asientos = "5",
                Color = "Blanco",
                Precio = 23950000m,
                Segmento = "Hatchback",
                Imagen =
                    "img/volkswagen.jpg"
            },


            new Vehiculo
            {
                Id = "cronos",
                Marca = "Fiat",
                Modelo = "Cronos",
                Anio = "2025",
                Motor = "1.3 GSE Firefly",
                Potencia = "99 CV",
                Transmision =
                    "Manual 5 velocidades",
                Traccion = "Delantera",
                Asientos = "5",
                Color = "Gris grafito",
                Precio = 28392000m,
                Segmento = "Sedán",
                Imagen = "img/cronos.jpg"
            },


            new Vehiculo
            {
                Id = "corolla",
                Marca = "Toyota",
                Modelo = "Corolla E210",
                Anio = "2024",
                Motor =
                    "2.0 Dynamic Force",
                Potencia = "170 CV",
                Transmision = "CVT",
                Traccion = "Delantera",
                Asientos = "5",
                Color = "Rojo",
                Precio = 48310000m,
                Segmento = "Sedán",
                Imagen = "img/toyota.jpg"
            },


            new Vehiculo
            {
                Id = "trailblazer",
                Marca = "Chevrolet",
                Modelo =
                    "Trailblazer High Country",
                Anio = "2024",
                Motor =
                    "2.8L turbodiésel",
                Potencia = "207 CV",
                Transmision =
                    "Automática 8 velocidades",
                Traccion = "4x4",
                Asientos = "5",
                Color = "Gris oscuro",
                Precio = 24041900m,
                Segmento = "SUV",
                Imagen =
                    "img/trailblazer.jpg"
            },


            new Vehiculo
            {
                Id = "mustangdh",
                Marca = "Ford",
                Modelo =
                    "Mustang GT Dark Horse",
                Anio = "2025",
                Motor =
                    "Coyote V8 5.0L",
                Potencia = "507 CV",
                Transmision =
                    "Automática 10 velocidades",
                Traccion = "Trasera",
                Asientos = "4",
                Color = "Gris Mercurio",
                Precio = 98225000m,
                Segmento = "Coupé",
                Imagen =
                    "img/dark-horse.jpg"
            }

        );
    }
}