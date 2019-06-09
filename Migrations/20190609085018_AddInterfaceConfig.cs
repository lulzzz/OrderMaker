using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mtd.OrderMaker.Web.Migrations
{
    public partial class AddInterfaceConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "timecr",
                table: "mtd_store",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "mtd_config_file",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(45)", nullable: false),
                    file_size = table.Column<string>(type: "varchar(45)", nullable: false),
                    file_type = table.Column<string>(type: "varchar(45)", nullable: false),
                    file_data = table.Column<byte[]>(type: "mediumblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mtd_config_file", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "id_UNIQUE",
                table: "mtd_config_file",
                column: "id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mtd_config_file");

            migrationBuilder.AlterColumn<DateTime>(
                name: "timecr",
                table: "mtd_store",
                type: "timestamp",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp",
                oldDefaultValueSql: "'CURRENT_TIMESTAMP'");
        }
    }
}
