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
                name: "Cards",
                columns: table => new
                {
                    adviceReference = table.Column<string>(type: "text", nullable: false),
                    token = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.adviceReference);
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
                name: "Payment",
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
                    processor = table.Column<string>(type: "text", nullable: true),
                    processor_message = table.Column<string>(type: "text", nullable: true),
                    authMode = table.Column<string>(type: "text", nullable: true),
                    authFields = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    account_number = table.Column<string>(type: "text", nullable: true),
                    account_name = table.Column<string>(type: "text", nullable: true),
                    account_balance = table.Column<double>(type: "double precision", nullable: false),
                    account_mandate = table.Column<double>(type: "double precision", nullable: false),
                    account_type = table.Column<string>(type: "text", nullable: true),
                    acccount_trackerRef = table.Column<string>(type: "text", nullable: true),
                    acccount_trackerId = table.Column<string>(type: "text", nullable: true),
                    wallet_provider = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "walletTransactionHistory",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    originator_accountNumber = table.Column<string>(type: "text", nullable: true),
                    destination_accountNumber = table.Column<string>(type: "text", nullable: true),
                    originator_accountName = table.Column<string>(type: "text", nullable: true),
                    destination_accountName = table.Column<string>(type: "text", nullable: true),
                    processor_reference = table.Column<string>(type: "text", nullable: true),
                    transaction_reference = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    narration = table.Column<string>(type: "text", nullable: true),
                    transaction_type = table.Column<string>(type: "text", nullable: true),
                    direction = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    transaction_date = table.Column<string>(type: "text", nullable: true),
                    provider = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_walletTransactionHistory", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auths");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "walletTransactionHistory");
        }
    }
}
