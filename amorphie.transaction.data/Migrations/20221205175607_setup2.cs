using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amorphie.transaction.data.Migrations
{
    /// <inheritdoc />
    public partial class setup2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataChecker_Transactions_TransactionDefinitionId",
                table: "DataChecker");

            migrationBuilder.DropUniqueConstraint(
                name: "Unique_OrderUrlTemplate",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "RequestUrlTemplate",
                table: "Transactions",
                newName: "StatusReason");

            migrationBuilder.RenameColumn(
                name: "OrderUrlTemplate",
                table: "Transactions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Client",
                table: "Transactions",
                newName: "RequestUpstreamResponse");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OrderRouteResponse",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderUpstreamResponse",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestRouteResponse",
                table: "Transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignalRHubToken",
                table: "Transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionDefinitionId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WorkflowId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Definitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestUrlTemplate = table.Column<string>(type: "text", nullable: false),
                    OrderUrlTemplate = table.Column<string>(type: "text", nullable: false),
                    Client = table.Column<string>(type: "text", nullable: false),
                    Workflow = table.Column<string>(type: "text", nullable: false),
                    TTL = table.Column<int>(type: "integer", nullable: false),
                    SignalRHub = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.Id);
                    table.UniqueConstraint("Unique_OrderUrlTemplate", x => new { x.OrderUrlTemplate, x.Client });
                });

            migrationBuilder.CreateTable(
                name: "TransactionLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromStatus = table.Column<string>(type: "text", nullable: false),
                    ToStatus = table.Column<string>(type: "text", nullable: false),
                    StatusReason = table.Column<string>(type: "text", nullable: false),
                    Log = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionLog_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDefinitionId",
                table: "Transactions",
                column: "TransactionDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLog_TransactionId",
                table: "TransactionLog",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataChecker_Definitions_TransactionDefinitionId",
                table: "DataChecker",
                column: "TransactionDefinitionId",
                principalTable: "Definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Definitions_TransactionDefinitionId",
                table: "Transactions",
                column: "TransactionDefinitionId",
                principalTable: "Definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataChecker_Definitions_TransactionDefinitionId",
                table: "DataChecker");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Definitions_TransactionDefinitionId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Definitions");

            migrationBuilder.DropTable(
                name: "TransactionLog");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionDefinitionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OrderRouteResponse",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "OrderUpstreamResponse",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RequestRouteResponse",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "SignalRHubToken",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionDefinitionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "StatusReason",
                table: "Transactions",
                newName: "RequestUrlTemplate");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Transactions",
                newName: "OrderUrlTemplate");

            migrationBuilder.RenameColumn(
                name: "RequestUpstreamResponse",
                table: "Transactions",
                newName: "Client");

            migrationBuilder.AddUniqueConstraint(
                name: "Unique_OrderUrlTemplate",
                table: "Transactions",
                columns: new[] { "OrderUrlTemplate", "Client" });

            migrationBuilder.AddForeignKey(
                name: "FK_DataChecker_Transactions_TransactionDefinitionId",
                table: "DataChecker",
                column: "TransactionDefinitionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
