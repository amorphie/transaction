using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class OrderType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "OrderUpstreamType",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("4b5e9645-c5aa-4daa-bba8-e1ea22c036c8"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("634c3b75-6e99-40ce-ae23-92c3080e258d"));

            migrationBuilder.DeleteData(
                table: "DataValidator",
                keyColumn: "Id",
                keyValue: new Guid("8a180276-ea6d-434f-bdc1-02834ca0391f"));

            migrationBuilder.DeleteData(
                table: "Definitions",
                keyColumn: "Id",
                keyValue: new Guid("9e336a53-1a3e-405c-ac08-2715e077d330"));

            migrationBuilder.DropColumn(
                name: "OrderUpstreamType",
                table: "Transactions");

            migrationBuilder.InsertData(
                table: "Definitions",
                columns: new[] { "Id", "Client", "OrderUrlMethod", "OrderUrlTemplate", "RequestUrlMethod", "RequestUrlTemplate", "TTL", "Workflow" },
                values: new object[] { new Guid("032164e8-9320-46c6-a0ae-565927d8334d"), "Web", 0, "/transfers/eft/execute", 0, "/transfers/eft/simulate", 600, "transaction-transfer-eft-over-web" });

            migrationBuilder.InsertData(
                table: "DataValidator",
                columns: new[] { "Id", "OrderDataPath", "RequestDataPath", "TransactionDefinitionId", "Type" },
                values: new object[,]
                {
                    { new Guid("1082e95a-ad71-4aca-af40-efb7be2ab063"), "$.target.name", "$.target.name", new Guid("032164e8-9320-46c6-a0ae-565927d8334d"), 0 },
                    { new Guid("2d4fcb00-405c-4bde-be71-cd4953e3535a"), "$.target.iban", "$.target.iban", new Guid("032164e8-9320-46c6-a0ae-565927d8334d"), 0 },
                    { new Guid("f87b8cea-3730-462a-9a22-67fba49b7280"), "$.amount.value", "$.amount.value", new Guid("032164e8-9320-46c6-a0ae-565927d8334d"), 1 }
                });
        }
    }
}
