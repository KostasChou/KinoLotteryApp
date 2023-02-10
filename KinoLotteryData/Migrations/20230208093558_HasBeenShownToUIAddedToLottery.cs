using Microsoft.EntityFrameworkCore.Migrations;

namespace KinoLotteryData.Migrations
{
    public partial class HasBeenShownToUIAddedToLottery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBeenShownToUI",
                table: "Lotteries",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBeenShownToUI",
                table: "Lotteries");
        }
    }
}
