using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OCPG.Migrations
{
    /// <inheritdoc />
    public partial class auths : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    access_token = table.Column<string>(type: "text", nullable: true),
                    expires_in = table.Column<long>(type: "bigint", nullable: false),
                    refreshtoken = table.Column<string>(type: "text", nullable: true),
                    timeGenerated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    LastName = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "paymentTransactions",
                columns: table => new
                {
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    AdviceReference = table.Column<string>(type: "text", nullable: true),
                    PaymentReference = table.Column<string>(type: "text", nullable: true),
                    Channel = table.Column<string>(type: "text", nullable: true),
                    MerchantName = table.Column<string>(type: "text", nullable: true),
                    MerchantCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paymentTransactions", x => x.Amount);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auths");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "paymentTransactions");
        }
    }
}
