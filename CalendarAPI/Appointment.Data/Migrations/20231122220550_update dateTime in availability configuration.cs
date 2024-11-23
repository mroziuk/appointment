using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Appointment.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatedateTimeinavailabilityconfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "User",
                type: "dateTime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Start",
                table: "Availabillity",
                type: "dateTime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "Availabillity",
                type: "dateTime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActiveTo",
                table: "Availabillity",
                type: "dateTime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActiveFrom",
                table: "Availabillity",
                type: "dateTime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "User",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "dateTime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Start",
                table: "Availabillity",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "dateTime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "End",
                table: "Availabillity",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "dateTime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActiveTo",
                table: "Availabillity",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "dateTime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActiveFrom",
                table: "Availabillity",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "dateTime2");
        }
    }
}
