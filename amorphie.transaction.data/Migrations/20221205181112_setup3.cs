using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class setup3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlTemplate", "RequestUrlTemplate", "SignalRHub", "TTL", "Workflow" },
                values: new object[] { new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70"), "Web", "/transfers/eft/execute", "/transfers/eft/simulate", "hub-transaction-transfer-over-web", 600, "transaction-transfer-over-web" });

            migrationBuilder.InsertData(
                table: "DataChecker",
                columns: new[] { "RequestDataPath", "TransactionDefinitionId", "OrderDataPath", "Type" },
                values: new object[,]
                {
                    { "$.amount.value", new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70"), "$.amount.value", 1 },
                    { "$.target.iban", new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70"), "$.target.iban", 0 },
                    { "$.target.name", new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70"), "$.target.name", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataChecker",
                keyColumns: new[] { "RequestDataPath", "TransactionDefinitionId" },
                keyValues: new object[] { "$.amount.value", new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70") });

            migrationBuilder.DeleteData(
                table: "DataChecker",
                keyColumns: new[] { "RequestDataPath", "TransactionDefinitionId" },
                keyValues: new object[] { "$.target.iban", new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70") });

            migrationBuilder.DeleteData(
                table: "DataChecker",
                keyColumns: new[] { "RequestDataPath", "TransactionDefinitionId" },
                keyValues: new object[] { "$.target.name", new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70") });

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("5f516f3a-2ca6-42dd-b63d-4cbdbca48f70"));
        }
    }
}
