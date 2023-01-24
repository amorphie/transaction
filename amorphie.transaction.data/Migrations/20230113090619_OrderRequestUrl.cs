using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class OrderRequestUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.AddColumn<string>(
                name: "OrderUpStreamUrl",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestUpStreamUrl",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("1082e95a-ad71-4aca-af40-efb7be2ab063"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("2d4fcb00-405c-4bde-be71-cd4953e3535a"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("f87b8cea-3730-462a-9a22-67fba49b7280"));

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("032164e8-9320-46c6-a0ae-565927d8334d"));

            migrationBuilder.DropColumn(
                name: "OrderUpStreamUrl",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RequestUpStreamUrl",
                table: "Transactions");

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
    }
}
