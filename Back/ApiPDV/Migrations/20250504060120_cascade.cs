using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiPDV.Migrations
{
    /// <inheritdoc />
    public partial class cascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutosCarrinho_Carrinhos_CarrinhoId",
                table: "ProdutosCarrinho");

            migrationBuilder.AlterColumn<int>(
                name: "CarrinhoId",
                table: "ProdutosCarrinho",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutosCarrinho_Carrinhos_CarrinhoId",
                table: "ProdutosCarrinho",
                column: "CarrinhoId",
                principalTable: "Carrinhos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdutosCarrinho_Carrinhos_CarrinhoId",
                table: "ProdutosCarrinho");

            migrationBuilder.AlterColumn<int>(
                name: "CarrinhoId",
                table: "ProdutosCarrinho",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdutosCarrinho_Carrinhos_CarrinhoId",
                table: "ProdutosCarrinho",
                column: "CarrinhoId",
                principalTable: "Carrinhos",
                principalColumn: "Id");
        }
    }
}
