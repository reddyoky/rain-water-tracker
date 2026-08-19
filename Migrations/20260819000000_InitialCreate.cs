using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WaterTracker.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "GlassTypes",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                CapacityMl = table.Column<int>(type: "integer", nullable: false),
                Emoji = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GlassTypes", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                Password = table.Column<string>(type: "text", nullable: false),
                DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                DailyGoalMl = table.Column<int>(type: "integer", nullable: false),
                ProfilePhotoPath = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "WaterLogs",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<int>(type: "integer", nullable: false),
                AmountMl = table.Column<int>(type: "integer", nullable: false),
                DrankAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WaterLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_WaterLogs_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "GlassTypes",
            columns: new[] { "Id", "Name", "CapacityMl", "Emoji" },
            values: new object[,]
            {
                { 1, "Küçük Bardak", 200, "🥛" },
                { 2, "Büyük Bardak", 300, "🥤" },
                { 3, "Şişe",         500, "💧" },
                { 4, "Büyük Şişe",   750, "🍶" }
            });

        migrationBuilder.CreateIndex(
            name: "IX_WaterLogs_UserId",
            table: "WaterLogs",
            column: "UserId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "WaterLogs");
        migrationBuilder.DropTable(name: "Users");
        migrationBuilder.DropTable(name: "GlassTypes");
    }
}
