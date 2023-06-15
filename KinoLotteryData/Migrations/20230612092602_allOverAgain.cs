using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KinoLotteryData.Migrations
{
    public partial class allOverAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lotteries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotteryDateTime = table.Column<DateTime>(nullable: false),
                    WinningNumbers = table.Column<string>(nullable: false),
                    MoneyPlayed = table.Column<int>(nullable: false),
                    MoneyWon = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lotteries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LotteryPerformances",
                columns: table => new
                {
                    LotteryPerformanceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfNumbers = table.Column<int>(nullable: false),
                    NumbersMatched = table.Column<int>(nullable: false),
                    PayoutMultiplier = table.Column<decimal>(type: "decimal(10, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotteryPerformances", x => x.LotteryPerformanceId);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(maxLength: 30, nullable: false),
                    Password = table.Column<byte[]>(maxLength: 500, nullable: false),
                    Salt = table.Column<byte[]>(nullable: false),
                    DoB = table.Column<DateTime>(nullable: false),
                    Wallet = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfNumbers = table.Column<int>(nullable: false),
                    NumbersPlayed = table.Column<string>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    MoneyPlayedPerLottery = table.Column<int>(nullable: false),
                    NumberOfLotteries = table.Column<int>(nullable: false),
                    RemainingLotteries = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LotteryTickets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LotteryId = table.Column<int>(nullable: false),
                    TicketId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotteryTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotteryTickets_Lotteries_LotteryId",
                        column: x => x.LotteryId,
                        principalTable: "Lotteries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LotteryTickets_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotteryTickets_LotteryId",
                table: "LotteryTickets",
                column: "LotteryId");

            migrationBuilder.CreateIndex(
                name: "IX_LotteryTickets_TicketId",
                table: "LotteryTickets",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_PlayerId",
                table: "Tickets",
                column: "PlayerId");

            // Seed data
            migrationBuilder.InsertData(
                table: "LotteryPerformances",
                columns: new[] { "NumberOfNumbers", "NumbersMatched", "PayoutMultiplier" },
                values: new object[,]
                {
                { 3, 0, 0 }, { 3, 1, 0 }, { 3, 2, 2.5f }, { 3, 3, 25 },
                { 4, 0, 0 }, { 4, 1, 0 }, { 4, 2, 1 }, { 4, 3, 4 }, { 4, 4, 100 },
                { 5, 0, 0 }, { 5, 1, 0 }, { 5, 2, 0 }, { 5, 3, 2 }, { 5, 4, 20 }, { 5, 5, 450 },
                { 6, 0, 0 }, { 6, 1, 0 }, { 6, 2, 0 }, { 6, 3, 1 }, { 6, 4, 7 }, { 6, 5, 50 }, { 6, 6, 1600 },
                { 7, 0, 0 }, { 7, 1, 0 }, { 7, 2, 0 }, { 7, 3, 1 }, { 7, 4, 3 }, { 7, 5, 20 }, { 7, 6, 100 }, { 7, 7, 5000 },
                { 8, 0, 0 }, { 8, 1, 0 }, { 8, 2, 0 }, { 8, 3, 0 }, { 8, 4, 2 }, { 8, 5, 10 }, { 8, 6, 50 }, { 8, 7, 1000 }, { 8, 8, 15000 },
                { 9, 0, 0 }, { 9, 1, 0 }, { 9, 2, 0 }, { 9, 3, 0 }, { 9, 4, 1 }, { 9, 5, 5 }, { 9, 6, 25 }, { 9, 7, 200 }, { 9, 8, 4000 }, { 9, 9, 40000 },
                { 10, 0, 2 }, { 10, 1, 0 }, { 10, 2, 0 }, { 10, 3, 0 }, { 10, 4, 0 }, { 10, 5, 2 }, { 10, 6, 20 }, { 10, 7, 80 }, { 10, 8, 500 }, { 10, 9, 10000 }, { 10, 10, 100000 },
                { 11, 0, 2 }, { 11, 1, 0 }, { 11, 2, 0 }, { 11, 3, 0 }, { 11, 4, 0 }, { 11, 5, 1 }, { 11, 6, 10 }, { 11, 7, 50 }, { 11, 8, 250 }, { 11, 9, 1500 }, { 11, 10, 15000 }, { 11, 11, 500000 },
                { 12, 0, 4 }, { 12, 1, 0 }, { 12, 2, 0 }, { 12, 3, 0 }, { 12, 4, 0 }, { 12, 5, 0 }, { 12, 6, 5 }, { 12, 7, 25 }, { 12, 8, 150 }, { 12, 9, 1000 }, { 12, 10, 2500 }, { 12, 11, 25000 }, { 12, 12, 1000000 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LotteryPerformances");

            migrationBuilder.DropTable(
                name: "LotteryTickets");

            migrationBuilder.DropTable(
                name: "Lotteries");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
