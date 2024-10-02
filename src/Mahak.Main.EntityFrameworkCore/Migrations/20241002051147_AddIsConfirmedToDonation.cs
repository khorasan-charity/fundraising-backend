using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mahak.Main.Migrations
{
    /// <inheritdoc />
    public partial class AddIsConfirmedToDonation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "AppDonations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "AppDonations");
        }
    }
}
