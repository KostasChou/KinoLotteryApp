using Microsoft.EntityFrameworkCore.Migrations;

namespace KinoLotteryData.Migrations
{
    public partial class apiUriTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "APIURIEntities",
                columns: table => new
                {
                    URIId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    URIString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_APIURIEntities", x => x.URIId);
                });

            migrationBuilder.InsertData(
                table: "APIURIEntities",
                columns: new[] { "URIId", "URIString" },
                values: new object[] { 1, "https://www.random.org/integers/?num=1&min=1&max=999999999&col=1&base=10&format=plain&rnd=new" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "APIURIEntities");
        }
    }
}
