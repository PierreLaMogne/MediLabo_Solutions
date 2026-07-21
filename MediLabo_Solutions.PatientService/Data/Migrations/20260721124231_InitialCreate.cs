using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediLabo_Solutions.PatientService.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Prénom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateDeNaissance = table.Column<DateOnly>(type: "date", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdressePostale = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NuméroDeTéléphone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
