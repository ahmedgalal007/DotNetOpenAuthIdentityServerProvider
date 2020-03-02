using Microsoft.EntityFrameworkCore.Migrations;

namespace IdentityServerProvider.Data.IDP
{
    public partial class InitiateIDPUserContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    Username = table.Column<string>(maxLength: 100, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    ClaimType = table.Column<string>(maxLength: 250, nullable: false),
                    ClaimValue = table.Column<string>(maxLength: 250, nullable: false),
                    UserSubjectId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_Users_UserSubjectId",
                        column: x => x.UserSubjectId,
                        principalTable: "Users",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 250, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 250, nullable: false),
                    UserSubjectId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserSubjectId",
                        column: x => x.UserSubjectId,
                        principalTable: "Users",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_UserSubjectId",
                table: "Claims",
                column: "UserSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserSubjectId",
                table: "UserLogins",
                column: "UserSubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
