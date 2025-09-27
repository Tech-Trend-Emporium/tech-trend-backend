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
                name: "ApprovalJobs",
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
                    table.PrimaryKey("PK_ApprovalJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalJobs_DecidedBy",
                table: "ApprovalJobs",
                column: "DecidedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalJobs_RequestedBy",
                table: "ApprovalJobs",
                column: "RequestedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalJobs_State_RequestedAt",
                table: "ApprovalJobs",
                columns: new[] { "State", "RequestedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalJobs");
        }
    }
}
