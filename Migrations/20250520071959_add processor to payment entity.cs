using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OCPG.Migrations
{
    /// <inheritdoc />
    public partial class addprocessortopaymententity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
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
                    isNotified = table.Column<bool>(type: "boolean", nullable: false),
                    processor = table.Column<string>(type: "text", nullable: true)
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
