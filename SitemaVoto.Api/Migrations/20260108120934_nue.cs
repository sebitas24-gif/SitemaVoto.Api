using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SitemaVoto.Api.Migrations
{
    /// <inheritdoc />
    public partial class nue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcesoElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: true),
                    TipoEleccion = table.Column<string>(type: "text", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
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
                    Cedula = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Genero = table.Column<string>(type: "text", nullable: false),
                    Correo = table.Column<string>(type: "text", nullable: false),
                    Canton = table.Column<string>(type: "text", nullable: false),
                    ImagenVerificacion = table.Column<string>(type: "text", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
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
                    IdOpcionElectoral = table.Column<int>(type: "integer", nullable: false),
                    VotoEncriptado = table.Column<string>(type: "text", nullable: true),
                    FechaVoto = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdVotante = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Administrador_Votantes_IdVotante",
                        column: x => x.IdVotante,
                        principalTable: "Votantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Candidato",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdVotante = table.Column<int>(type: "integer", nullable: false),
                    Partido = table.Column<string>(type: "text", nullable: true),
                    Eslogan = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidato", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidato_Votantes_IdVotante",
                        column: x => x.IdVotante,
                        principalTable: "Votantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Papeletas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdVotante = table.Column<int>(type: "integer", nullable: false),
                    IdProcesoElectoral = table.Column<int>(type: "integer", nullable: false),
                    CodigoConfirmacion = table.Column<string>(type: "text", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    VotanteId = table.Column<int>(type: "integer", nullable: false),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Papeletas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Papeletas_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Papeletas_Votantes_VotanteId",
                        column: x => x.VotanteId,
                        principalTable: "Votantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpcionElectorales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProcesoElectoral = table.Column<int>(type: "integer", nullable: false),
                    IdCandidato = table.Column<int>(type: "integer", nullable: false),
                    NombreOpcion = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CandidatoId = table.Column<int>(type: "integer", nullable: true),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpcionElectorales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpcionElectorales_Candidato_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidato",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpcionElectorales_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HistorialResultados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProcesoElectoral = table.Column<int>(type: "integer", nullable: false),
                    IdOpcionGanadora = table.Column<int>(type: "integer", nullable: true),
                    VotosGanador = table.Column<int>(type: "integer", nullable: false),
                    TotalVotosProcesoElectoral = table.Column<int>(type: "integer", nullable: false),
                    PorcentajeVictoria = table.Column<double>(type: "double precision", nullable: false),
                    FechaConsolidacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OpcionGanadoraId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialResultados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialResultados_OpcionElectorales_OpcionGanadoraId",
                        column: x => x.OpcionGanadoraId,
                        principalTable: "OpcionElectorales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistorialResultados_ProcesoElectorales_IdProcesoElectoral",
                        column: x => x.IdProcesoElectoral,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultadoOpcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProceso = table.Column<int>(type: "integer", nullable: false),
                    IdOpcion = table.Column<int>(type: "integer", nullable: false),
                    TotalVotos = table.Column<int>(type: "integer", nullable: false),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: true),
                    OpcionElectoralId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultadoOpcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultadoOpcion_OpcionElectorales_OpcionElectoralId",
                        column: x => x.OpcionElectoralId,
                        principalTable: "OpcionElectorales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResultadoOpcion_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrador_IdVotante",
                table: "Administrador",
                column: "IdVotante",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidato_IdVotante",
                table: "Candidato",
                column: "IdVotante",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialResultados_IdProcesoElectoral",
                table: "HistorialResultados",
                column: "IdProcesoElectoral",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialResultados_OpcionGanadoraId",
                table: "HistorialResultados",
                column: "OpcionGanadoraId");

            migrationBuilder.CreateIndex(
                name: "IX_OpcionElectorales_CandidatoId",
                table: "OpcionElectorales",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_OpcionElectorales_ProcesoElectoralId",
                table: "OpcionElectorales",
                column: "ProcesoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_Papeletas_ProcesoElectoralId",
                table: "Papeletas",
                column: "ProcesoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_Papeletas_VotanteId",
                table: "Papeletas",
                column: "VotanteId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadoOpcion_OpcionElectoralId",
                table: "ResultadoOpcion",
                column: "OpcionElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultadoOpcion_ProcesoElectoralId",
                table: "ResultadoOpcion",
                column: "ProcesoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ProcesoElectoralId",
                table: "Votos",
                column: "ProcesoElectoralId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "HistorialResultados");

            migrationBuilder.DropTable(
                name: "Papeletas");

            migrationBuilder.DropTable(
                name: "ResultadoOpcion");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "OpcionElectorales");

            migrationBuilder.DropTable(
                name: "Candidato");

            migrationBuilder.DropTable(
                name: "ProcesoElectorales");

            migrationBuilder.DropTable(
                name: "Votantes");
        }
    }
}
