using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inventory8.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserIdShadowColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_request_users_UserId",
                table: "request");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_audit_users_UserId",
                table: "stock_audit");

            migrationBuilder.DropIndex(
                name: "IX_stock_audit_UserId",
                table: "stock_audit");

            migrationBuilder.DropIndex(
                name: "IX_request_UserId",
                table: "request");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "stock_audit");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "request");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "stock_audit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "request",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_audit_UserId",
                table: "stock_audit",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_request_UserId",
                table: "request",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_request_users_UserId",
                table: "request",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_audit_users_UserId",
                table: "stock_audit",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }
    }
}
