using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SitemaVoto.Api.Migrations
{
    /// <inheritdoc />
    public partial class ini : Migration
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
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    InicioLocal = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FinLocal = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcesoElectorales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidatos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false),
                    NombreCompleto = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    Partido = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Binomio = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    NumeroLista = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidatos_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Votos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false),
                    CandidatoId = table.Column<int>(type: "integer", nullable: true),
                    Provincia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Canton = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    EmitidoUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    HashIntegridad = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votos_Candidatos_CandidatoId",
                        column: x => x.CandidatoId,
                        principalTable: "Candidatos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Votos_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    Accion = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Entidad = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    EntidadId = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ip = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auditorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CodigoPadrones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    EmitidoPorUsuarioId = table.Column<int>(type: "integer", nullable: true),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    EmitidoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Usado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CodigoPadrones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CodigoPadrones_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Juntas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoMesa = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Provincia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Canton = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    JefeJuntaUsuarioId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Juntas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cedula = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombres = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Apellidos = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Correo = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Rol = table.Column<int>(type: "integer", nullable: false),
                    Provincia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Canton = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Parroquia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    JuntaId = table.Column<int>(type: "integer", nullable: true),
                    ImagenUrl = table.Column<string>(type: "text", nullable: true),
                    HabilitadoLegalmente = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Juntas_JuntaId",
                        column: x => x.JuntaId,
                        principalTable: "Juntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OtpSesiones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    Metodo = table.Column<int>(type: "integer", nullable: false),
                    ExpiraUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Usado = table.Column<bool>(type: "boolean", nullable: false),
                    IntentosFallidos = table.Column<int>(type: "integer", nullable: false),
                    CreadoUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpSesiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtpSesiones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParticipacionVotantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcesoElectoralId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    CodigoComprobante = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    EmitidoUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Provincia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    Canton = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CodigoMesa = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipacionVotantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipacionVotantes_ProcesoElectorales_ProcesoElectoralId",
                        column: x => x.ProcesoElectoralId,
                        principalTable: "ProcesoElectorales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipacionVotantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilVotantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Provincia = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Canton = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    Mesa = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilVotantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilVotantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_ActorUsuarioId",
                table: "Auditorias",
                column: "ActorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatos_ProcesoElectoralId",
                table: "Candidatos",
                column: "ProcesoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_CodigoPadrones_EmitidoPorUsuarioId",
                table: "CodigoPadrones",
                column: "EmitidoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CodigoPadrones_ProcesoElectoralId",
                table: "CodigoPadrones",
                column: "ProcesoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_CodigoPadrones_UsuarioId_ProcesoElectoralId",
                table: "CodigoPadrones",
                columns: new[] { "UsuarioId", "ProcesoElectoralId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Juntas_JefeJuntaUsuarioId",
                table: "Juntas",
                column: "JefeJuntaUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_OtpSesiones_UsuarioId",
                table: "OtpSesiones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionVotantes_ProcesoElectoralId",
                table: "ParticipacionVotantes",
                column: "ProcesoElectoralId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipacionVotantes_UsuarioId_ProcesoElectoralId",
                table: "ParticipacionVotantes",
                columns: new[] { "UsuarioId", "ProcesoElectoralId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilVotantes_UsuarioId",
                table: "PerfilVotantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_JuntaId",
                table: "Usuarios",
                column: "JuntaId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_CandidatoId",
                table: "Votos",
                column: "CandidatoId");

            migrationBuilder.CreateIndex(
                name: "IX_Votos_ProcesoElectoralId",
                table: "Votos",
                column: "ProcesoElectoralId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auditorias_Usuarios_ActorUsuarioId",
                table: "Auditorias",
                column: "ActorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CodigoPadrones_Usuarios_EmitidoPorUsuarioId",
                table: "CodigoPadrones",
                column: "EmitidoPorUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CodigoPadrones_Usuarios_UsuarioId",
                table: "CodigoPadrones",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Juntas_Usuarios_JefeJuntaUsuarioId",
                table: "Juntas",
                column: "JefeJuntaUsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Juntas_Usuarios_JefeJuntaUsuarioId",
                table: "Juntas");

            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "CodigoPadrones");

            migrationBuilder.DropTable(
                name: "OtpSesiones");

            migrationBuilder.DropTable(
                name: "ParticipacionVotantes");

            migrationBuilder.DropTable(
                name: "PerfilVotantes");

            migrationBuilder.DropTable(
                name: "Votos");

            migrationBuilder.DropTable(
                name: "Candidatos");

            migrationBuilder.DropTable(
                name: "ProcesoElectorales");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Juntas");
        }
    }
}
