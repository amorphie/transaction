using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class setup7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("5b1a67ea-db00-44ec-b7dc-7ece385cf98d"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("6cc173f6-6d31-46a2-9d7a-d018f3b3f063"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("f765ff39-18fe-4312-b965-2ec7e6f18e9c"));

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("98479237-2230-483a-894b-0d10af4e8086"));

            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlTemplate", "RequestUrlTemplate", "SignalRHub", "TTL", "Workflow" },
                values: new object[] { new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"), "Web", "/transfers/eft/execute", "/transfers/eft/simulate", "hub-transaction-transfer-eft-over-web", 600, "transaction-transfer-eft-over-web" });

            migrationBuilder.InsertData(
                table: "DataValidator",
                columns: new[] { "Id", "OrderDataPath", "RequestDataPath", "TransactionDefinitionId", "Type" },
                values: new object[,]
                {
                    { new Guid("7ed0628a-d400-42d5-b286-47865993ebc0"), "$.target.name", "$.target.name", new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"), 0 },
                    { new Guid("de2881e4-0b03-4410-b9f7-1cfaf7514e66"), "$.target.iban", "$.target.iban", new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"), 0 },
                    { new Guid("e145fba6-455a-4c5f-b5a6-37ec9a09c542"), "$.amount.value", "$.amount.value", new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"), 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("7ed0628a-d400-42d5-b286-47865993ebc0"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("de2881e4-0b03-4410-b9f7-1cfaf7514e66"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("e145fba6-455a-4c5f-b5a6-37ec9a09c542"));

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("5f28ef33-03e7-43cc-8987-c1734c434a32"));

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
        }
    }
}
