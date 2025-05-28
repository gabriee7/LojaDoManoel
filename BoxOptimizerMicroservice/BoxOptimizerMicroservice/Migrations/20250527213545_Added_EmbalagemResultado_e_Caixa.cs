using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoxOptimizerMicroservice.Migrations
{
    /// <inheritdoc />
    public partial class Added_EmbalagemResultado_e_Caixa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caixa",
                columns: table => new
                {
                    _id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Altura = table.Column<int>(type: "int", nullable: false),
                    Comprimento = table.Column<int>(type: "int", nullable: false),
                    Largura = table.Column<int>(type: "int", nullable: false),
                    NomeCaixa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caixa", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "EmbalagemResultado",
                columns: table => new
                {
                    _id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    ProdutosNestaCaixa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoCaixaUsadaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbalagemResultado", x => x._id);
                    table.ForeignKey(
                        name: "FK_EmbalagemResultado_Caixa_TipoCaixaUsadaId",
                        column: x => x.TipoCaixaUsadaId,
                        principalTable: "Caixa",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Caixa",
                columns: new[] { "_id", "LastModifiedTime", "Altura", "Comprimento", "CreationTime", "Largura", "NomeCaixa" },
                values: new object[,]
                {
                    { new Guid("b5d9e8c1-6a2f-4b0d-8a3e-1f9c8b7a6d5e"), null, 80, 40, new DateTime(2025, 5, 27, 12, 0, 0, 0, DateTimeKind.Utc), 50, "Caixa 2" },
                    { new Guid("c9f0b7a2-5e1d-4c9a-b2d8-2a0e1c9f8b7d"), null, 50, 60, new DateTime(2025, 5, 27, 12, 0, 0, 0, DateTimeKind.Utc), 80, "Caixa 3" },
                    { new Guid("e2a8c9f0-7d3b-4a1e-9c6a-08d7e5b4c3f2"), null, 30, 80, new DateTime(2025, 5, 27, 12, 0, 0, 0, DateTimeKind.Utc), 40, "Caixa 1" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Caixa_NomeCaixa",
                table: "Caixa",
                column: "NomeCaixa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmbalagemResultado_TipoCaixaUsadaId",
                table: "EmbalagemResultado",
                column: "TipoCaixaUsadaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmbalagemResultado");

            migrationBuilder.DropTable(
                name: "Caixa");
        }
    }
}
