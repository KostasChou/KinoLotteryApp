using Microsoft.EntityFrameworkCore.Migrations;

namespace KinoLotteryData.Migrations
{
    public partial class NumbersMatched_MoneyWonColumnsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MoneyWon",
                table: "LotteryTickets",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "NumbersMatched",
                table: "LotteryTickets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneyWon",
                table: "LotteryTickets");

            migrationBuilder.DropColumn(
                name: "NumbersMatched",
                table: "LotteryTickets");
        }
    }
}
