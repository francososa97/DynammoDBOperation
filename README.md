# DynamoDB Operations with .NET

This project demonstrates how to interact with AWS DynamoDB using .NET, showcasing various operations like creating tables, loading data, querying, updating, and using high-level API tasks. The project is designed to provide a clear understanding of DynamoDB operations and serves as an example for building similar applications.

## Features

- **Create Table**: Dynamically create a DynamoDB table with specified attributes and throughput.
- **Load Data**: Load data into the DynamoDB table from a JSON file.
- **Query Data**: Query data based on partition keys using DynamoDB's query capabilities.
- **Update Data**: Update specific attributes of items in the table conditionally.
- **High-Level API Usage**: Demonstrate high-level object persistence model operations using DynamoDB context.
- **Paginate Data**: Example of paginated queries for handling large datasets.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- AWS account with permissions to use DynamoDB
- AWS credentials configured on your local machine
- Basic knowledge of .NET and AWS DynamoDB

## Setup

1. Clone the repository:

    ```bash
    git clone https://github.com/yourusername/DynamoDBOperations.git
    cd DynamoDBOperations
    ```

2. Install the required dependencies:

    ```bash
    dotnet add package Microsoft.Extensions.Configuration
    dotnet add package Microsoft.Extensions.Configuration.Json
    dotnet add package AWSSDK.DynamoDBv2
    dotnet add package AWSSDK.Extensions.NETCore.Setup
    dotnet add package System.Text.Json
    ```

3. Add your AWS credentials by configuring the AWS CLI or adding them to the `appsettings.json` if required.

4. Update the `appsettings.json` file with your DynamoDB table configuration:

    ```json
    {
      "DynamoDB": {
        "TableName": "Notes",
        "PartitionKey": "UserId",
        "SortKey": "NoteId",
        "ReadCapacity": 5,
        "WriteCapacity": 5,
        "Sourcenotes": "notes.json",
        "QueryUserId": "testuser",
        "PageSize": 10,
        "QueryNoteId": 1,
        "NotePrefix": "Updated Note"
      }
    }
    ```

5. Ensure the `notes.json` file is present in the root directory with sample data for loading into the DynamoDB table.

## Usage

To run the project, use the following command and select the desired operation when prompted:

```bash
dotnet run
