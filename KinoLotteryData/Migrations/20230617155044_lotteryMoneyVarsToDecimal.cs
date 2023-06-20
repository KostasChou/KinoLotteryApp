using Microsoft.EntityFrameworkCore.Migrations;

namespace KinoLotteryData.Migrations
{
    public partial class lotteryMoneyVarsToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MoneyWon",
                table: "Lotteries",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "MoneyPlayed",
                table: "Lotteries",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MoneyWon",
                table: "Lotteries",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "MoneyPlayed",
                table: "Lotteries",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
