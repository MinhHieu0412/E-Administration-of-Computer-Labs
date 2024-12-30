using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Administration.Migrations
{
    /// <inheritdoc />
    public partial class h7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_Departments_DepartmentID",
                table: "IssueReports");

            migrationBuilder.RenameColumn(
                name: "DepartmentID",
                table: "IssueReports",
                newName: "DepartmentsID");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReports_DepartmentID",
                table: "IssueReports",
                newName: "IX_IssueReports_DepartmentsID");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentsID1",
                table: "IssueReports",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IssueReports_DepartmentsID1",
                table: "IssueReports",
                column: "DepartmentsID1");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_Department",
                table: "IssueReports",
                column: "DepartmentsID",
                principalTable: "Departments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_Departments_DepartmentsID1",
                table: "IssueReports",
                column: "DepartmentsID1",
                principalTable: "Departments",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_Department",
                table: "IssueReports");

            migrationBuilder.DropForeignKey(
                name: "FK_IssueReports_Departments_DepartmentsID1",
                table: "IssueReports");

            migrationBuilder.DropIndex(
                name: "IX_IssueReports_DepartmentsID1",
                table: "IssueReports");

            migrationBuilder.DropColumn(
                name: "DepartmentsID1",
                table: "IssueReports");

            migrationBuilder.RenameColumn(
                name: "DepartmentsID",
                table: "IssueReports",
                newName: "DepartmentID");

            migrationBuilder.RenameIndex(
                name: "IX_IssueReports_DepartmentsID",
                table: "IssueReports",
                newName: "IX_IssueReports_DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_IssueReports_Departments_DepartmentID",
                table: "IssueReports",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
