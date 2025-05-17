using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OCPG.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

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
                name: "payment",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    merchantCode = table.Column<string>(type: "text", nullable: true),
                    adviceReference = table.Column<string>(type: "text", nullable: true),
                    paymentReference = table.Column<string>(type: "text", nullable: true),
                    merchantReference = table.Column<string>(type: "text", nullable: true),
                    amountCollected = table.Column<double>(type: "double precision", nullable: false),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    transactionStatus = table.Column<string>(type: "text", nullable: true),
                    currencyCode = table.Column<string>(type: "text", nullable: true),
                    accountNumberMasked = table.Column<string>(type: "text", nullable: true),
                    narration = table.Column<string>(type: "text", nullable: true),
                    customerName = table.Column<string>(type: "text", nullable: true),
                    paymentDate = table.Column<string>(type: "text", nullable: true),
                    requestPayload = table.Column<string>(type: "text", nullable: true),
                    responsePayload = table.Column<string>(type: "text", nullable: true),
                    channel = table.Column<string>(type: "text", nullable: true),
                    notificationUrl = table.Column<string>(type: "text", nullable: true),
                    callbackUrl = table.Column<string>(type: "text", nullable: true),
                    isNotified = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.id);
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
                name: "payment");
        }
    }
}
