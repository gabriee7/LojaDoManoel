using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoxOptimizerMicroservice.Migrations
{
    /// <inheritdoc />
    public partial class Added_ClientApplication_e_Seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AplicacaoCliente",
                columns: table => new
                {
                    _id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashedApiKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AplicacaoCliente", x => x._id);
                });

            migrationBuilder.InsertData(
                table: "AplicacaoCliente",
                columns: new[] { "_id", "LastModifiedTime", "CreationTime", "HashedApiKey", "Nome" },
                values: new object[] { new Guid("2a8badd0-2a0a-4b79-b51f-9e7a0e2b8c3d"), null, new DateTime(2025, 5, 27, 12, 0, 0, 0, DateTimeKind.Utc), "61fIzNFUoKv3An80F60QB/bJrDAw2H+B+uwvRegT644=", "Aplicação Cliente via Seed" });

            migrationBuilder.CreateIndex(
                name: "IX_AplicacaoCliente_Nome",
                table: "AplicacaoCliente",
                column: "Nome",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AplicacaoCliente");
        }
    }
}
