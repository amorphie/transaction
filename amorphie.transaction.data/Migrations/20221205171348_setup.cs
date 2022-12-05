using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class setup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestUrlTemplate = table.Column<string>(type: "text", nullable: false),
                    OrderUrlTemplate = table.Column<string>(type: "text", nullable: false),
                    Client = table.Column<string>(type: "text", nullable: false),
                    Workflow = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.UniqueConstraint("Unique_OrderUrlTemplate", x => new { x.OrderUrlTemplate, x.Client });
                });

            migrationBuilder.CreateTable(
                name: "DataChecker",
                columns: table => new
                {
                    RequestDataPath = table.Column<string>(type: "text", nullable: false),
                    TransactionDefinitionId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrderDataPath = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataChecker", x => x.RequestDataPath);
                    table.ForeignKey(
                        name: "FK_DataChecker_Transactions_TransactionDefinitionId",
                        column: x => x.TransactionDefinitionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataChecker_TransactionDefinitionId",
                table: "DataChecker",
                column: "TransactionDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataChecker");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
