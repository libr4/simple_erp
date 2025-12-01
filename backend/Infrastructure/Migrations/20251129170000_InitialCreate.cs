using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produto",
                columns: table => new
                {
                    Codigo = table.Column<long>(type: "bigint", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Estoque = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produto", x => x.Codigo);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacaoEstoque",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    CodigoProduto = table.Column<long>(type: "bigint", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    SaldoAntes = table.Column<int>(type: "int", nullable: false),
                    SaldoDepois = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacaoEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movimentacao_Produto",
                        column: x => x.CodigoProduto,
                        principalTable: "Produto",
                        principalColumn: "Codigo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacao_Codigo_DataHoraDesc",
                table: "MovimentacaoEstoque",
                columns: new[] { "CodigoProduto", "DataHora" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacaoEstoque_PublicId",
                table: "MovimentacaoEstoque",
                column: "PublicId",
                unique: true);

            // Seed initial data
            migrationBuilder.InsertData(
                table: "Produto",
                columns: new[] { "Codigo", "Descricao", "Estoque" },
                values: new object[,]
                {
                    { 101L, "Caneta Azul", 150 },
                    { 102L, "Caderno Universitário", 75 },
                    { 103L, "Borracha Branca", 200 },
                    { 104L, "Lápis Preto HB", 320 },
                    { 105L, "Marcador de Texto Amarelo", 90 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimentacaoEstoque");

            migrationBuilder.DropTable(
                name: "Produto");
        }
    }
}
