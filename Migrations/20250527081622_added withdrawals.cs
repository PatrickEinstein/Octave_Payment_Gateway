using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OCPG.Migrations
{
    /// <inheritdoc />
    public partial class addedwithdrawals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
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
                    wallet_provider = table.Column<int>(type: "integer", nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactionHistory",
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
                    table.PrimaryKey("PK_WalletTransactionHistory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Withdrawals",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    wallet_accountNumber = table.Column<string>(type: "text", nullable: true),
                    bank_accountNumber = table.Column<string>(type: "text", nullable: true),
                    amount = table.Column<double>(type: "double precision", nullable: false),
                    narration = table.Column<string>(type: "text", nullable: true),
                    transactionReference = table.Column<string>(type: "text", nullable: true),
                    processorRef = table.Column<string>(type: "text", nullable: true),
                    processorMsg = table.Column<string>(type: "text", nullable: true),
                    currency = table.Column<bool>(type: "boolean", nullable: false),
                    channelCode = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawals", x => x.id);
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
                name: "WalletTransactionHistory");

            migrationBuilder.DropTable(
                name: "Withdrawals");
        }
    }
}
