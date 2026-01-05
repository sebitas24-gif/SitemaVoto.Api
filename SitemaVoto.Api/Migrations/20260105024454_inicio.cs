using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SitemaVoto.Api.Migrations
{
    /// <inheritdoc />
    public partial class inicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TipoEvento = table.Column<string>(type: "text", nullable: true),
                    EstadoProceso = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OpcionElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Partido = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    ImagenVerificacion = table.Column<string>(type: "text", nullable: true),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpcionElectorales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Papeletas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CodigoConfirmacion = table.Column<string>(type: "text", nullable: true),
                    VotanteId = table.Column<int>(type: "integer", nullable: false),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Papeletas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcesoElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcesoElectorales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "text", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    Apellido = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EstaHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    YaVoto = table.Column<bool>(type: "boolean", nullable: false),
                    ImagenVerificacion = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OpcionElectoralId = table.Column<int>(type: "integer", nullable: false),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false),
                    VotoEncriptado = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "OpcionElectorales");

            migrationBuilder.DropTable(
                name: "Papeletas");

            migrationBuilder.DropTable(
                name: "ProcesoElectorales");

            migrationBuilder.DropTable(
                name: "Votantes");

            migrationBuilder.DropTable(
                name: "Votos");
        }
    }
}
