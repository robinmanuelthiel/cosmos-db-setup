# Azure Cosmos DB Setup Container

Instantiate the Azure Cosmos DB by creating databases and containers

[![adasd](https://img.shields.io/badge/Docker_Hub-robinmanuelthiel/cosmos--db--setup:latest-blue?logo=docker)](https://hub.docker.com/r/robinmanuelthiel/cosmos-db-setup/)

```bash
docker run --rm robinmanuelthiel/speedtest:latest -- "{\"databaseName\": \"test\", \"containers\": [{\"name\": \"mycontainer\", \"partitionKey\": \"/id\"}]}"
```

## Configuration

To configure the container, you need to provide a JSON configuration as an argument.

```json
{
  "databaseName": "test",
  "containers": [
    {
      "name": "mycontainer",
      "partitionKey": "/id"
    }
  ]
}
```

Additionally, you can set the following environment variables:

| Environment Variable | Description | Default |
| -- | -- | -- |
| `CONNECTION_STRING` | | `AccountEndpoint=https://cosmosdb:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==` |
| `DELAY` | The delay in seconds to wait until start seeding | `0` |

## Examples

### Setup Cosmos DB Emulator with Docker Compose

```yaml
version: '3.6'
services:
  setup:
    image: robinmanuelthiel/cosmos-db-setup:latest
    command:
      - "{\"databaseName\": \"test\", \"containers\": [{\"name\": \"mycontainer\", \"partitionKey\": \"/id\"}]}"
    depends_on:
      - cosmosdb

  cosmosdb:
    image: mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
    hostname: cosmosdb
    container_name: cosmosdb
    mem_limit: 3g
    cpus: 2.0
    ports:
      - '8081:8081'
      - '10251:10251'
      - '10252:10252'
      - '10254:10254'
    environment:
      - AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10
      - AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true
```
