using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventBooking.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventPhotoEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventPhoto_Events_EventId",
                table: "EventPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventPhoto",
                table: "EventPhoto");

            migrationBuilder.RenameTable(
                name: "EventPhoto",
                newName: "EventPhotos");

            migrationBuilder.RenameIndex(
                name: "IX_EventPhoto_EventId",
                table: "EventPhotos",
                newName: "IX_EventPhotos_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventPhotos",
                table: "EventPhotos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventPhotos_Events_EventId",
                table: "EventPhotos",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventPhotos_Events_EventId",
                table: "EventPhotos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventPhotos",
                table: "EventPhotos");

            migrationBuilder.RenameTable(
                name: "EventPhotos",
                newName: "EventPhoto");

            migrationBuilder.RenameIndex(
                name: "IX_EventPhotos_EventId",
                table: "EventPhoto",
                newName: "IX_EventPhoto_EventId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventPhoto",
                table: "EventPhoto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventPhoto_Events_EventId",
                table: "EventPhoto",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
