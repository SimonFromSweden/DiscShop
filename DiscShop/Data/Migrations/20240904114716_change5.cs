using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class change5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Disc_DiscId1",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_DiscId1",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "DiscId1",
                table: "Cart");

            migrationBuilder.AlterColumn<int>(
                name: "DiscId",
                table: "Cart",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cart_DiscId",
                table: "Cart",
                column: "DiscId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Disc_DiscId",
                table: "Cart",
                column: "DiscId",
                principalTable: "Disc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Disc_DiscId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_DiscId",
                table: "Cart");

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

            migrationBuilder.CreateIndex(
                name: "IX_Cart_DiscId1",
                table: "Cart",
                column: "DiscId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Disc_DiscId1",
                table: "Cart",
                column: "DiscId1",
                principalTable: "Disc",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
