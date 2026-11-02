using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace blazornacho.Migrations
{
    /// <inheritdoc />
    public partial class CreacionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Apellido = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false, collation: "NOCASE"),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    CodigoRecuperacionHash = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehiculos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Anio = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Motor = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Potencia = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Transmision = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Traccion = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Asientos = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Segmento = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Imagen = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehiculos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reclamos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Seguro = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Patente = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Siniestro = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reclamos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reclamos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Turnos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Hora = table.Column<string>(type: "TEXT", maxLength: 5, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Motivo = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    FechaReserva = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turnos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turnos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resenas",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VehiculoId = table.Column<string>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<long>(type: "INTEGER", nullable: false),
                    Puntaje = table.Column<int>(type: "INTEGER", nullable: false),
                    Comentario = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resenas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resenas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resenas_Vehiculos_VehiculoId",
                        column: x => x.VehiculoId,
                        principalTable: "Vehiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Vehiculos",
                columns: new[] { "Id", "Anio", "Asientos", "Color", "Imagen", "Marca", "Modelo", "Motor", "Potencia", "Precio", "Segmento", "Traccion", "Transmision" },
                values: new object[,]
                {
                    { "corolla", "2024", "5", "Rojo", "img/toyota.jpg", "Toyota", "Corolla E210", "2.0 Dynamic Force", "170 CV", 48310000m, "Sedán", "Delantera", "CVT" },
                    { "cronos", "2025", "5", "Gris grafito", "img/cronos.jpg", "Fiat", "Cronos", "1.3 GSE Firefly", "99 CV", 28392000m, "Sedán", "Delantera", "Manual 5 velocidades" },
                    { "f150", "2025", "5", "Negro Ebony", "img/ford.jpg", "Ford", "F-150 Trenor", "5.0L Coyote V8", "406 CV", 135375000m, "Pick-up", "4x4", "Automática 10 velocidades" },
                    { "golfgti", "2023", "5", "Blanco", "img/volkswagen.jpg", "Volkswagen", "Golf GTI", "2.0 TSI turbo", "241 hp", 23950000m, "Hatchback", "Delantera", "DSG 7 velocidades" },
                    { "mustangdh", "2025", "4", "Gris Mercurio", "img/dark-horse.jpg", "Ford", "Mustang GT Dark Horse", "Coyote V8 5.0L", "507 CV", 98225000m, "Coupé", "Trasera", "Automática 10 velocidades" },
                    { "trailblazer", "2024", "5", "Gris oscuro", "img/trailblazer.jpg", "Chevrolet", "Trailblazer High Country", "2.8L turbodiésel", "207 CV", 24041900m, "SUV", "4x4", "Automática 8 velocidades" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reclamos_UsuarioId",
                table: "Reclamos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Resenas_UsuarioId",
                table: "Resenas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Resenas_VehiculoId",
                table: "Resenas",
                column: "VehiculoId");

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_Fecha_Hora",
                table: "Turnos",
                columns: new[] { "Fecha", "Hora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Turnos_UsuarioId",
                table: "Turnos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reclamos");

            migrationBuilder.DropTable(
                name: "Resenas");

            migrationBuilder.DropTable(
                name: "Turnos");

            migrationBuilder.DropTable(
                name: "Vehiculos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
