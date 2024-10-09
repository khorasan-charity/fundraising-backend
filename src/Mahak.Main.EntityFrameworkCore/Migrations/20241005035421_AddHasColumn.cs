using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mahak.Main.Migrations
{
    /// <inheritdoc />
    public partial class AddHasColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Hash",
                table: "AppDonations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "AppDonations",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "AppDonations");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "AppDonations");
        }
    }
}
