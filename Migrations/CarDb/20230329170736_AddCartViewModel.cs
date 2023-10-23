using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRent.Migrations.CarDb
{
    /// <inheritdoc />
    public partial class AddCartViewModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
     name: "Sales",
     columns: table => new
     {
         Id = table.Column<int>(nullable: false)
             .Annotation("SqlServer:Identity", "1, 1"), 
         UserId = table.Column<string>(nullable: true),
         SaleDate = table.Column<DateTime>(nullable: false),
         TotalPrice = table.Column<decimal>(nullable: false)
     },
     constraints: table =>
     {
         table.PrimaryKey("PK_Sales", x => x.Id); 
     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Sales");
        }
    }
}
