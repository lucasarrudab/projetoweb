using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiPDV.Migrations
{
    /// <inheritdoc />
    public partial class teste5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_MetodosPagamento_MetodoPagamentoId",
                table: "Vendas");

            migrationBuilder.AlterColumn<int>(
                name: "MetodoPagamentoId",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "MetodosPagamento",
                columns: new[] { "Id", "Nome" },
                values: new object[,]
                {
                    { 1, "Dinheiro" },
                    { 2, "Débito" },
                    { 3, "Crédito" },
                    { 4, "PIX" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_MetodosPagamento_MetodoPagamentoId",
                table: "Vendas",
                column: "MetodoPagamentoId",
                principalTable: "MetodosPagamento",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_MetodosPagamento_MetodoPagamentoId",
                table: "Vendas");

            migrationBuilder.DeleteData(
                table: "MetodosPagamento",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MetodosPagamento",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MetodosPagamento",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MetodosPagamento",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.AlterColumn<int>(
                name: "MetodoPagamentoId",
                table: "Vendas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_MetodosPagamento_MetodoPagamentoId",
                table: "Vendas",
                column: "MetodoPagamentoId",
                principalTable: "MetodosPagamento",
                principalColumn: "Id");
        }
    }
}
