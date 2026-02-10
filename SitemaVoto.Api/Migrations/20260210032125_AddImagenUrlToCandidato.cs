using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SitemaVoto.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddImagenUrlToCandidato : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Candidatos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Candidatos");
        }
    }
}
