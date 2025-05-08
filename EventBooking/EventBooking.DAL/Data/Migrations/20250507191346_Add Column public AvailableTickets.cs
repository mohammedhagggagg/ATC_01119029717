using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventBooking.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnpublicAvailableTickets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableTickets",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableTickets",
                table: "Events");
        }
    }
}
