using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class RequestBody : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "OrderUpstreamBody",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestUpstreamBody",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlMethod", "OrderUrlTemplate", "RequestUrlMethod", "RequestUrlTemplate", "TTL", "Workflow" },
                values: new object[] { new Guid("071d60eb-4b24-4102-8589-1b05467eff23"), "Web", 0, "/transfers/eft/execute", 0, "/transfers/eft/simulate", 600, "transaction-transfer-eft-over-web" });

            migrationBuilder.InsertData(
                table: "DataValidator",
                columns: new[] { "Id", "OrderDataPath", "RequestDataPath", "TransactionDefinitionId", "Type" },
                values: new object[,]
                {
                    { new Guid("855e32f1-267f-4e91-ad84-dbcfbbd9ffbb"), "$.amount.value", "$.amount.value", new Guid("071d60eb-4b24-4102-8589-1b05467eff23"), 1 },
                    { new Guid("a99578ce-449a-44db-bf46-86036ab80e9b"), "$.target.name", "$.target.name", new Guid("071d60eb-4b24-4102-8589-1b05467eff23"), 0 },
                    { new Guid("b6820978-3aa0-41a6-a059-2e469900c13e"), "$.target.iban", "$.target.iban", new Guid("071d60eb-4b24-4102-8589-1b05467eff23"), 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("855e32f1-267f-4e91-ad84-dbcfbbd9ffbb"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("a99578ce-449a-44db-bf46-86036ab80e9b"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("b6820978-3aa0-41a6-a059-2e469900c13e"));

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("071d60eb-4b24-4102-8589-1b05467eff23"));

            migrationBuilder.DropColumn(
                name: "OrderUpstreamBody",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RequestUpstreamBody",
                table: "Transactions");

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
    }
}
