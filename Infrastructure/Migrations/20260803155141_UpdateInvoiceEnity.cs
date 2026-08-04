using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoicesService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoiceEnity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Invoices_invoiceId",
                table: "InvoiceItems");

            migrationBuilder.RenameColumn(
                name: "total",
                table: "Invoices",
                newName: "Total");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Invoices",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "customerId",
                table: "Invoices",
                newName: "CustomerId");

            migrationBuilder.RenameColumn(
                name: "currency",
                table: "Invoices",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "createdAt",
                table: "Invoices",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Invoices",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "unitPrice",
                table: "InvoiceItems",
                newName: "UnitPrice");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "InvoiceItems",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "productId",
                table: "InvoiceItems",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "invoiceId",
                table: "InvoiceItems",
                newName: "InvoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceItems_invoiceId",
                table: "InvoiceItems",
                newName: "IX_InvoiceItems_InvoiceId");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "Invoices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_Invoices_InvoiceId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "Total",
                table: "Invoices",
                newName: "total");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Invoices",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Invoices",
                newName: "customerId");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "Invoices",
                newName: "currency");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Invoices",
                newName: "createdAt");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Invoices",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "InvoiceItems",
                newName: "unitPrice");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "InvoiceItems",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "InvoiceItems",
                newName: "productId");

            migrationBuilder.RenameColumn(
                name: "InvoiceId",
                table: "InvoiceItems",
                newName: "invoiceId");

            migrationBuilder.RenameIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                newName: "IX_InvoiceItems_invoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_Invoices_invoiceId",
                table: "InvoiceItems",
                column: "invoiceId",
                principalTable: "Invoices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
