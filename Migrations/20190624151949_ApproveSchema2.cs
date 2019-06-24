using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class ApproveSchema2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_log_approval",
                table: "mtd_log_approval");

            migrationBuilder.DropIndex(
                name: "fk_log_approve_idx",
                table: "mtd_log_approval");

            migrationBuilder.DropColumn(
                name: "mtd_approval",
                table: "mtd_log_approval");

            migrationBuilder.AddColumn<sbyte>(
                name: "complete",
                table: "mtd_store_approval",
                type: "tinyint(4)",
                nullable: false,
                defaultValueSql: "'0'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "complete",
                table: "mtd_store_approval");

            migrationBuilder.AddColumn<string>(
                name: "mtd_approval",
                table: "mtd_log_approval",
                type: "varchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "fk_log_approve_idx",
                table: "mtd_log_approval",
                column: "mtd_approval");

            migrationBuilder.AddForeignKey(
                name: "fk_log_approval",
                table: "mtd_log_approval",
                column: "mtd_approval",
                principalTable: "mtd_approval",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
