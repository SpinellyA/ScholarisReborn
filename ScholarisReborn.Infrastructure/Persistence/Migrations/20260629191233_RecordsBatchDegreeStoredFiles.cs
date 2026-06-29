using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScholarisReborn.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RecordsBatchDegreeStoredFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProofOfRegistrationURL",
                table: "TermRecordProofOfRegistrations");

            migrationBuilder.DropColumn(
                name: "TrueCopyOfGradesURL",
                table: "TermRecordGradeTranscripts");

            migrationBuilder.AddColumn<bool>(
                name: "GradesRequired",
                table: "TermRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "TermRecordProofOfRegistrations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "TermRecordGradeTranscripts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "StipendDrops",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StipendDrops",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BatchNumber",
                table: "Scholars",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DegreeProgram",
                table: "Scholars",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "BatchNumber",
                table: "Invitations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DegreeProgram",
                table: "Invitations",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoredFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: false),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoredFiles");

            migrationBuilder.DropColumn(
                name: "GradesRequired",
                table: "TermRecords");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "TermRecordProofOfRegistrations");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "TermRecordGradeTranscripts");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "StipendDrops");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StipendDrops");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "Scholars");

            migrationBuilder.DropColumn(
                name: "DegreeProgram",
                table: "Scholars");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "DegreeProgram",
                table: "Invitations");

            migrationBuilder.AddColumn<string>(
                name: "ProofOfRegistrationURL",
                table: "TermRecordProofOfRegistrations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TrueCopyOfGradesURL",
                table: "TermRecordGradeTranscripts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
