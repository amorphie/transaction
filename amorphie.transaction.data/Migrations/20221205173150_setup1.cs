using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class setup1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataChecker_Transactions_TransactionDefinitionId",
                table: "DataChecker");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DataChecker",
                table: "DataChecker");

            migrationBuilder.DropIndex(
                name: "IX_DataChecker_TransactionDefinitionId",
                table: "DataChecker");

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionDefinitionId",
                table: "DataChecker",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataChecker",
                table: "DataChecker",
                columns: new[] { "TransactionDefinitionId", "RequestDataPath" });

            migrationBuilder.AddForeignKey(
                name: "FK_DataChecker_Transactions_TransactionDefinitionId",
                table: "DataChecker",
                column: "TransactionDefinitionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataChecker_Transactions_TransactionDefinitionId",
                table: "DataChecker");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DataChecker",
                table: "DataChecker");

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionDefinitionId",
                table: "DataChecker",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataChecker",
                table: "DataChecker",
                column: "RequestDataPath");

            migrationBuilder.CreateIndex(
                name: "IX_DataChecker_TransactionDefinitionId",
                table: "DataChecker",
                column: "TransactionDefinitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataChecker_Transactions_TransactionDefinitionId",
                table: "DataChecker",
                column: "TransactionDefinitionId",
                principalTable: "Transactions",
                principalColumn: "Id");
        }
    }
}
