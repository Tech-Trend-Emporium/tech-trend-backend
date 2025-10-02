using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Persistence.Migrations.Governance
{
    /// <inheritdoc />
    public partial class InitGovernance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approval_jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Operation = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<bool>(type: "boolean", nullable: false),
                    RequestedBy = table.Column<int>(type: "integer", nullable: false),
                    DecidedBy = table.Column<int>(type: "integer", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now() at time zone 'utc'"),
                    DecidedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_jobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_approval_jobs_DecidedBy",
                table: "approval_jobs",
                column: "DecidedBy");

            migrationBuilder.CreateIndex(
                name: "IX_approval_jobs_RequestedBy",
                table: "approval_jobs",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_approval_jobs_State_RequestedAt",
                table: "approval_jobs",
                columns: new[] { "State", "RequestedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_jobs");
        }
    }
}
