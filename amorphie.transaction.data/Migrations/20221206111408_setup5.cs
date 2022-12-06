using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class setup5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataComparer");

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("a1a7a3a9-03bc-4fdc-ae46-0115f4cf69e5"));

            migrationBuilder.CreateTable(
                name: "DataValidator",
                columns: table => new
                {
                    TransactionDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestDataPath = table.Column<string>(type: "text", nullable: false),
                    OrderDataPath = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataValidator", x => new { x.TransactionDefinitionId, x.RequestDataPath });
                    table.ForeignKey(
                        name: "FK_DataValidator_Definitions_TransactionDefinitionId",
                        column: x => x.TransactionDefinitionId,
                        principalTable: "Definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlTemplate", "RequestUrlTemplate", "SignalRHub", "TTL", "Workflow" },
                values: new object[] { new Guid("4548b5c4-e827-405e-b833-048783076d4b"), "Web", "/transfers/eft/execute", "/transfers/eft/simulate", "hub-transaction-transfer-eft-over-web", 600, "transaction-transfer-eft-over-web" });

            migrationBuilder.InsertData(
                table: "DataValidator",
                columns: new[] { "RequestDataPath", "TransactionDefinitionId", "OrderDataPath", "Type" },
                values: new object[,]
                {
                    { "$.amount.value", new Guid("4548b5c4-e827-405e-b833-048783076d4b"), "$.amount.value", 1 },
                    { "$.target.iban", new Guid("4548b5c4-e827-405e-b833-048783076d4b"), "$.target.iban", 0 },
                    { "$.target.name", new Guid("4548b5c4-e827-405e-b833-048783076d4b"), "$.target.name", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataValidator");

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("4548b5c4-e827-405e-b833-048783076d4b"));

            migrationBuilder.CreateTable(
                name: "DataComparer",
                columns: table => new
                {
                    TransactionDefinitionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestDataPath = table.Column<string>(type: "text", nullable: false),
                    OrderDataPath = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataComparer", x => new { x.TransactionDefinitionId, x.RequestDataPath });
                    table.ForeignKey(
                        name: "FK_DataComparer_Definitions_TransactionDefinitionId",
                        column: x => x.TransactionDefinitionId,
                        principalTable: "Definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlTemplate", "RequestUrlTemplate", "SignalRHub", "TTL", "Workflow" },
                values: new object[] { new Guid("a1a7a3a9-03bc-4fdc-ae46-0115f4cf69e5"), "Web", "/transfers/eft/execute", "/transfers/eft/simulate", "hub-transaction-transfer-eft-over-web", 600, "transaction-transfer-eft-over-web" });

            migrationBuilder.InsertData(
                table: "DataComparer",
                columns: new[] { "RequestDataPath", "TransactionDefinitionId", "OrderDataPath", "Type" },
                values: new object[,]
                {
                    { "$.amount.value", new Guid("a1a7a3a9-03bc-4fdc-ae46-0115f4cf69e5"), "$.amount.value", 1 },
                    { "$.target.iban", new Guid("a1a7a3a9-03bc-4fdc-ae46-0115f4cf69e5"), "$.target.iban", 0 },
                    { "$.target.name", new Guid("a1a7a3a9-03bc-4fdc-ae46-0115f4cf69e5"), "$.target.name", 0 }
                });
        }
    }
}
