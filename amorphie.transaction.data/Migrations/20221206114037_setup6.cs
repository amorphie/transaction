using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class setup6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataValidator",
                table: "DataValidator");

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumns: new[] { "RequestDataPath", "TransactionDefinitionId" },
                keyValues: new object[] { "$.amount.value", new Guid("4548b5c4-e827-405e-b833-048783076d4b") });

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumns: new[] { "RequestDataPath", "TransactionDefinitionId" },
                keyValues: new object[] { "$.target.iban", new Guid("4548b5c4-e827-405e-b833-048783076d4b") });

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumns: new[] { "RequestDataPath", "TransactionDefinitionId" },
                keyValues: new object[] { "$.target.name", new Guid("4548b5c4-e827-405e-b833-048783076d4b") });

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("4548b5c4-e827-405e-b833-048783076d4b"));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "DataValidator",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataValidator",
                table: "DataValidator",
                column: "Id");

            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlTemplate", "RequestUrlTemplate", "SignalRHub", "TTL", "Workflow" },
                values: new object[] { new Guid("98479237-2230-483a-894b-0d10af4e8086"), "Web", "/transfers/eft/execute", "/transfers/eft/simulate", "hub-transaction-transfer-eft-over-web", 600, "transaction-transfer-eft-over-web" });

            migrationBuilder.InsertData(
                table: "DataValidator",
                columns: new[] { "Id", "OrderDataPath", "RequestDataPath", "TransactionDefinitionId", "Type" },
                values: new object[,]
                {
                    { new Guid("5b1a67ea-db00-44ec-b7dc-7ece385cf98d"), "$.target.iban", "$.target.iban", new Guid("98479237-2230-483a-894b-0d10af4e8086"), 0 },
                    { new Guid("6cc173f6-6d31-46a2-9d7a-d018f3b3f063"), "$.target.name", "$.target.name", new Guid("98479237-2230-483a-894b-0d10af4e8086"), 0 },
                    { new Guid("f765ff39-18fe-4312-b965-2ec7e6f18e9c"), "$.amount.value", "$.amount.value", new Guid("98479237-2230-483a-894b-0d10af4e8086"), 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataValidator_TransactionDefinitionId",
                table: "DataValidator",
                column: "TransactionDefinitionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataValidator",
                table: "DataValidator");

            migrationBuilder.DropIndex(
                name: "IX_DataValidator_TransactionDefinitionId",
                table: "DataValidator");

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyColumnType: "uuid",
                keyValue: new Guid("5b1a67ea-db00-44ec-b7dc-7ece385cf98d"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyColumnType: "uuid",
                keyValue: new Guid("6cc173f6-6d31-46a2-9d7a-d018f3b3f063"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyColumnType: "uuid",
                keyValue: new Guid("f765ff39-18fe-4312-b965-2ec7e6f18e9c"));

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("98479237-2230-483a-894b-0d10af4e8086"));

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DataValidator");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataValidator",
                table: "DataValidator",
                columns: new[] { "TransactionDefinitionId", "RequestDataPath" });

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
    }
}
