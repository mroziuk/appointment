using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment.Data.Migrations
{
    /// <inheritdoc />
    public partial class adduserresettoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetToken",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetToken",
                table: "User");
        }
    }
}
