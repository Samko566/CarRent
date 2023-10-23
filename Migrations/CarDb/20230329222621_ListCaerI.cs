using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRent.Migrations.CarDb
{
    /// <inheritdoc />
    public partial class ListCaerI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sales",
                table: "Sales",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CarId",
                table: "CartItems",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Car_CarId",
                table: "CartItems",
                column: "CarId",
                principalTable: "Car",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Car_CarId",
                table: "CartItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sales",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_CarId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Sales");
        }
    }
}
