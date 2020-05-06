using Microsoft.EntityFrameworkCore.Migrations;

namespace ForbiddenBooks.Migrations
{
    public partial class ForbiddenBooksDBv11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Markets_MarketId",
                table: "Magazines");

            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Users_UserId",
                table: "Magazines");

            migrationBuilder.DropIndex(
                name: "IX_Magazines_MarketId",
                table: "Magazines");

            migrationBuilder.DropIndex(
                name: "IX_Magazines_UserId",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "MarketId",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Magazines");

            migrationBuilder.AddColumn<int>(
                name: "marketOwnerId",
                table: "Magazines",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "userOwnerId",
                table: "Magazines",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Magazines_marketOwnerId",
                table: "Magazines",
                column: "marketOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Magazines_userOwnerId",
                table: "Magazines",
                column: "userOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Markets_marketOwnerId",
                table: "Magazines",
                column: "marketOwnerId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Users_userOwnerId",
                table: "Magazines",
                column: "userOwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Markets_marketOwnerId",
                table: "Magazines");

            migrationBuilder.DropForeignKey(
                name: "FK_Magazines_Users_userOwnerId",
                table: "Magazines");

            migrationBuilder.DropIndex(
                name: "IX_Magazines_marketOwnerId",
                table: "Magazines");

            migrationBuilder.DropIndex(
                name: "IX_Magazines_userOwnerId",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "marketOwnerId",
                table: "Magazines");

            migrationBuilder.DropColumn(
                name: "userOwnerId",
                table: "Magazines");

            migrationBuilder.AddColumn<int>(
                name: "MarketId",
                table: "Magazines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Magazines",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Magazines_MarketId",
                table: "Magazines",
                column: "MarketId");

            migrationBuilder.CreateIndex(
                name: "IX_Magazines_UserId",
                table: "Magazines",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Markets_MarketId",
                table: "Magazines",
                column: "MarketId",
                principalTable: "Markets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Magazines_Users_UserId",
                table: "Magazines",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
