using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityLearningAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewUserInvitationsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewUserInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InvitationToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvitatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewUserInvitations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewUserInvitations");
        }
    }
}
