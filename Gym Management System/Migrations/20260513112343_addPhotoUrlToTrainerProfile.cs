using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gym_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class addPhotoUrlToTrainerProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "TrainerProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "TrainerProfiles");
        }
    }
}
