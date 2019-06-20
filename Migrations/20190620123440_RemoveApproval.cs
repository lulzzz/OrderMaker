using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class RemoveApproval : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "approval",
                table: "mtd_form_part");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "approval",
                table: "mtd_form_part",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'0'");
        }
    }
}
