using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class change3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_AspNetUsers_UserId1",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Disc_DiscId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_DiscId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_UserId1",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Cart",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DiscId",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "DiscId1",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_DiscId1",
                table: "Cart",
                column: "DiscId1");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_UserId",
                table: "Cart",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_AspNetUsers_UserId",
                table: "Cart",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Disc_DiscId1",
                table: "Cart",
                column: "DiscId1",
                principalTable: "Disc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_AspNetUsers_UserId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Disc_DiscId1",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_DiscId1",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_UserId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "DiscId1",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DiscId",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Cart",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_DiscId",
                table: "Cart",
                column: "DiscId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Disc_DiscId",
                table: "Cart",
                column: "DiscId",
                principalTable: "Disc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
