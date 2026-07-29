using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoicesService.Migrations
{
    /// <inheritdoc />
    public partial class SettingManyToOnerelationbetweeninvoiceandinvoiceItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_invoiceItems_invoiceId",
                table: "invoiceItems",
                column: "invoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_invoiceItems_Invoices_invoiceId",
                table: "invoiceItems",
                column: "invoiceId",
                principalTable: "Invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_invoiceItems_Invoices_invoiceId",
                table: "invoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_invoiceItems_invoiceId",
                table: "invoiceItems");
        }
    }
}
