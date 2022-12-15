using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class defupd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "SignalRHub",
                table: "Definitions");

            migrationBuilder.AddColumn<int>(
                name: "OrderUrlMethod",
                table: "Definitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestUrlMethod",
                table: "Definitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlMethod", "OrderUrlTemplate", "RequestUrlMethod", "RequestUrlTemplate", "TTL", "Workflow" },
                values: new object[] { new Guid("6b15d033-8718-4977-b863-59bd0fff458c"), "Web", 0, "/transfers/eft/execute", 0, "/transfers/eft/simulate", 600, "transaction-transfer-eft-over-web" });

            migrationBuilder.InsertData(
                table: "DataValidator",
                columns: new[] { "Id", "OrderDataPath", "RequestDataPath", "TransactionDefinitionId", "Type" },
                values: new object[,]
                {
                    { new Guid("788e05e5-6ede-4b15-89f4-7a3d6c8c43c1"), "$.target.name", "$.target.name", new Guid("6b15d033-8718-4977-b863-59bd0fff458c"), 0 },
                    { new Guid("bfb3fe7d-a7d3-4b72-aa20-c2354b12f682"), "$.amount.value", "$.amount.value", new Guid("6b15d033-8718-4977-b863-59bd0fff458c"), 1 },
                    { new Guid("fc680460-76e2-4aee-80ca-750d78d3579d"), "$.target.iban", "$.target.iban", new Guid("6b15d033-8718-4977-b863-59bd0fff458c"), 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("788e05e5-6ede-4b15-89f4-7a3d6c8c43c1"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("bfb3fe7d-a7d3-4b72-aa20-c2354b12f682"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("fc680460-76e2-4aee-80ca-750d78d3579d"));

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("6b15d033-8718-4977-b863-59bd0fff458c"));

            migrationBuilder.DropColumn(
                name: "OrderUrlMethod",
                table: "Definitions");

            migrationBuilder.DropColumn(
                name: "RequestUrlMethod",
                table: "Definitions");

            migrationBuilder.AddColumn<string>(
                name: "SignalRHub",
                table: "Definitions",
                type: "text",
                nullable: false,
                defaultValue: "");

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
    }
}
