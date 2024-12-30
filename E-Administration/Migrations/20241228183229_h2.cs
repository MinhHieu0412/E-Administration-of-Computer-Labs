using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Administration.Migrations
{
    /// <inheritdoc />
    public partial class h2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RepairAssignments_IssueReports_IssueReportID",
                table: "RepairAssignments");

            migrationBuilder.DropTable(
                name: "LabRequestDto");

            migrationBuilder.DropIndex(
                name: "IX_RepairAssignments_IssueReportID",
                table: "RepairAssignments");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "RepairAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "ELearning",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "ELearning",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.CreateIndex(
                name: "IX_RepairAssignments_IssueReportID",
                table: "RepairAssignments",
                column: "IssueReportID");

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_EquipmentID",
                table: "IssueReports",
                column: "EquipmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_Equipments_EquipmentID",
                table: "IssueReports",
                column: "EquipmentID",
                principalTable: "Equipments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairAssignments_IssueReports_IssueReportID",
                table: "RepairAssignments",
                column: "IssueReportID",
                principalTable: "IssueReports",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_Equipments_EquipmentID",
                table: "IssueReports");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairAssignments_IssueReports_IssueReportID",
                table: "RepairAssignments");

            migrationBuilder.DropIndex(
                name: "IX_RepairAssignments_IssueReportID",
                table: "RepairAssignments");

            migrationBuilder.DropIndex(
                name: "IX_IssueReports_EquipmentID",
                table: "IssueReports");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "RepairAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "ELearning",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "ELearning",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "LabRequestDto",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestedByID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabRequestDto", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RepairAssignments_IssueReportID",
                table: "RepairAssignments",
                column: "IssueReportID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairAssignments_IssueReports_IssueReportID",
                table: "RepairAssignments",
                column: "IssueReportID",
                principalTable: "IssueReports",
                principalColumn: "ID");
        }
    }
}
