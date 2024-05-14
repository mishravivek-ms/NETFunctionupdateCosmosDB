# .NET Azure Function with EventHub Project

## Overview
This project is centered around the development of a **.NET Azure Function**. This function is architected to be triggered by **EventHub**, a robust big data streaming platform and event ingestion service provided by Azure.

## Functionality
The Azure Function springs into action when invoked by EventHub. It reads the messages from the EventHub which are rich in data that needs to be processed and stored.

## Data Storage
The Azure Function is responsible for passing the data from these messages into two distinct storage systems: **CosmosDB** and a **SQL Server Stored Procedure**.

### CosmosDB
CosmosDB is a globally distributed, multi-model database service provided by Azure for managing data at a planetary scale. The Azure Function passes the data into CosmosDB, ensuring reliable and scalable storage.

### SQL Server Stored Procedure
The Azure Function also interacts with a **Stored Procedure** in SQL Server. A Stored Procedure is a prepared SQL code that you can save and reuse. In this case, the Azure Function passes the data into this Stored Procedure, allowing for efficient data management and manipulation within the SQL Server.

This architecture ensures a seamless flow of data from EventHub to CosmosDB and SQL Server, effectively managing and storing data. This .NET Azure Function serves as a key component in this data processing pipeline.


### Local Testing
Create a **local.setting.json** file on the root location and add following fields 
```json
{
        "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "eventhubconnection": "<EVENTHUB CONNECTION URL>",
        "Cosmos_DB_Connection_String": "<COSMOS DB URL>",
        "SqlServerConnectionString": "<SQL SERVER CONNECTION URL>"
    }
}
```

### Flow Diagram
![Architectural diagram for the baseline scenario.](/media/Flowchar.jpg)
