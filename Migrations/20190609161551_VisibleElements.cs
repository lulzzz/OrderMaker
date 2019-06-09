using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class VisibleElements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "visible_date",
                table: "mtd_form",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'1'");

            migrationBuilder.AddColumn<sbyte>(
                name: "visible_number",
                table: "mtd_form",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "visible_date",
                table: "mtd_form");

            migrationBuilder.DropColumn(
                name: "visible_number",
                table: "mtd_form");
        }
    }
}
