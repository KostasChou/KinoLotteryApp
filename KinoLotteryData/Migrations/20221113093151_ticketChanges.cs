using Microsoft.EntityFrameworkCore.Migrations;

namespace KinoLotteryData.Migrations
{
    public partial class ticketChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumbersPlayer",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLotteries",
                table: "Tickets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NumbersPlayed",
                table: "Tickets",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfLotteries",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "NumbersPlayed",
                table: "Tickets");

            migrationBuilder.AddColumn<string>(
                name: "NumbersPlayer",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
