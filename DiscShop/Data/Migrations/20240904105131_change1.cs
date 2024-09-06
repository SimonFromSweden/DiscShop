using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class change1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Cart",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId1",
                table: "Cart",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_AspNetUsers_UserId1",
                table: "Cart",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_AspNetUsers_UserId1",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_UserId1",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Cart");
        }
    }
}
