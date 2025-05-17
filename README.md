# Payment Gateway - Robust Payment Processing System

## Overview

This project is a comprehensive payment gateway solution that provides multiple payment channels to facilitate seamless transactions. It is designed to handle a variety of payment methods, including:

* Card Payments
* Bank Transfers
* Direct Debits
* USSD Transactions
* Wallet Payments

The gateway is robust, scalable, and optimized for secure financial transactions, making it a versatile choice for merchants looking to integrate reliable payment processing.

## Features

* **Multiple Payment Channels:** Accept payments via card, bank transfer, direct debit, USSD, and wallets.
* **Transaction Management:** Monitor and manage transactions with real-time status updates.
* **Security:** Implemented with strong encryption and secure data handling practices.
* **Scalability:** Optimized to handle high volumes of transactions with minimal latency.
* **Integration Support:** Easy integration with third-party applications and merchant platforms.

## Tech Stack

* **Backend:** .NET 5, PostgreSQL, Dapper
* **Frontend:** React, TailwindCSS
* **Messaging/Queueing:** RabbitMQ
* **APIs:** RESTful APIs with OpenAPI documentation
* **Task Scheduling:** Cron Jobs

## Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/PatrickEinstein/Octave_Payment_Gateway.git
   ```

2. Navigate to the project directory:

   ```bash
   cd OCPG
   ```

3. Install dependencies:

   ```bash
   dotnet restore
   npm install
   ```

4. Set up the database:

   * Update the database connection string in the `appsettings.json` file.
   * Apply database migrations:

   ```bash
   dotnet ef database update
   ```

5. Start the server:

   ```bash
   dotnet run
   ```

## Configuration

* Update environment variables in the `.env` file.
* Configure RabbitMQ, Redis, and PostgreSQL as required.

## Usage

* Register as a merchant to obtain API keys.
* Use the provided endpoints to initiate payments via card, bank transfer, direct debit, USSD, or wallet.
* Monitor transaction status through the dashboard.

## Testing

Run the test suite using:

```bash
dotnet test
```

## Contributing

Contributions are welcome! Please fork the repository and create a pull request with your proposed changes.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more information.

## Contact

For support or inquiries, please contact `mohammedola1234@gmail.com`.
