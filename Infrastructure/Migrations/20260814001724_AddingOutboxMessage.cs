using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoicesService.Migrations
{
    /// <inheritdoc />
    public partial class AddingOutboxMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                newName: "OutboxMessages");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_OutboxId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_InboxMessageId_InboxConsumerId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_ExpirationTime");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessages",
                newName: "IX_OutboxMessages_EnqueueTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages",
                column: "SequenceNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessages_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessages",
                columns: new[] { "InboxMessageId", "InboxConsumerId" },
                principalTable: "InboxState",
                principalColumns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessages_OutboxState_OutboxId",
                table: "OutboxMessages",
                column: "OutboxId",
                principalTable: "OutboxState",
                principalColumn: "OutboxId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessages_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_OutboxMessages_OutboxState_OutboxId",
                table: "OutboxMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessages",
                table: "OutboxMessages");

            migrationBuilder.RenameTable(
                name: "OutboxMessages",
                newName: "OutboxMessage");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_OutboxId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_ExpirationTime",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_ExpirationTime");

            migrationBuilder.RenameIndex(
                name: "IX_OutboxMessages_EnqueueTime",
                table: "OutboxMessage",
                newName: "IX_OutboxMessage_EnqueueTime");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "SequenceNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId" },
                principalTable: "InboxState",
                principalColumns: new[] { "MessageId", "ConsumerId" });

            migrationBuilder.AddForeignKey(
                name: "FK_OutboxMessage_OutboxState_OutboxId",
                table: "OutboxMessage",
                column: "OutboxId",
                principalTable: "OutboxState",
                principalColumn: "OutboxId");
        }
    }
}
