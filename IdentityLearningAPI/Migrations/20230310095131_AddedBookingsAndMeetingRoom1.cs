using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityLearningAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedBookingsAndMeetingRoom1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_MeetingRoom_MeetingId",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingRoom",
                table: "MeetingRoom");

            migrationBuilder.RenameTable(
                name: "MeetingRoom",
                newName: "MeetingRooms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingRooms",
                table: "MeetingRooms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_MeetingRooms_MeetingId",
                table: "Bookings",
                column: "MeetingId",
                principalTable: "MeetingRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_MeetingRooms_MeetingId",
                table: "Bookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MeetingRooms",
                table: "MeetingRooms");

            migrationBuilder.RenameTable(
                name: "MeetingRooms",
                newName: "MeetingRoom");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MeetingRoom",
                table: "MeetingRoom",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_MeetingRoom_MeetingId",
                table: "Bookings",
                column: "MeetingId",
                principalTable: "MeetingRoom",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
