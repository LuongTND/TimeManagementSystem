using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackingSystem.Migrations
{
    public partial class addDBcontext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clock_Accounts_AccountId",
                table: "Clock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clock",
                table: "Clock");

            migrationBuilder.RenameTable(
                name: "Clock",
                newName: "Clocks");

            migrationBuilder.RenameIndex(
                name: "IX_Clock_AccountId",
                table: "Clocks",
                newName: "IX_Clocks_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clocks",
                table: "Clocks",
                column: "ClockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clocks_Accounts_AccountId",
                table: "Clocks",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clocks_Accounts_AccountId",
                table: "Clocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clocks",
                table: "Clocks");

            migrationBuilder.RenameTable(
                name: "Clocks",
                newName: "Clock");

            migrationBuilder.RenameIndex(
                name: "IX_Clocks_AccountId",
                table: "Clock",
                newName: "IX_Clock_AccountId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clock",
                table: "Clock",
                column: "ClockId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clock_Accounts_AccountId",
                table: "Clock",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
