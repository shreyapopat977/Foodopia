using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Foodopia.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminForeignKeyToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Admin_ID",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Admin_ID",
                table: "Products",
                column: "Admin_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Admins_Admin_ID",
                table: "Products",
                column: "Admin_ID",
                principalTable: "Admins",
                principalColumn: "Admin_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Admins_Admin_ID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_Admin_ID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Admin_ID",
                table: "Products");
        }
    }
}
