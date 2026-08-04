using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoicesService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoiceItems_Invoices_invoiceId",
                table: "invoiceItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_invoiceItems",
                table: "invoiceItems");

            migrationBuilder.RenameTable(
                name: "invoiceItems",
                newName: "InvoiceItems");

            migrationBuilder.RenameIndex(
                name: "IX_invoiceItems_invoiceId",
                table: "InvoiceItems",
                newName: "IX_InvoiceItems_invoiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InvoiceItems",
                table: "InvoiceItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Invoices_invoiceId",
                table: "InvoiceItems",
                column: "invoiceId",
                principalTable: "Invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Invoices_invoiceId",
                table: "InvoiceItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InvoiceItems",
                table: "InvoiceItems");

            migrationBuilder.RenameTable(
                name: "InvoiceItems",
                newName: "invoiceItems");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceItems_invoiceId",
                table: "invoiceItems",
                newName: "IX_invoiceItems_invoiceId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invoiceItems",
                table: "invoiceItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_invoiceItems_Invoices_invoiceId",
                table: "invoiceItems",
                column: "invoiceId",
                principalTable: "Invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
