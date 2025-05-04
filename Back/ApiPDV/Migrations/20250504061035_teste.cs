using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPDV.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorTotalCarrinho",
                table: "Carrinhos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ValorTotalCarrinho",
                table: "Carrinhos",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
