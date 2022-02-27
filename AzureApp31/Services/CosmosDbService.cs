using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureApp;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace AzureApp31.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private Container _container;

        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task AddItemAsync(Person item)
        {
            await this._container.CreateItemAsync<Person>(item, new PartitionKey(item.id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await this._container.DeleteItemAsync<Person>(id, new PartitionKey(id));
        }

        public async Task<Person> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Person> response = await this._container.ReadItemAsync<Person>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

        }

        public async Task<IEnumerable<Person>> GetItemsAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<Person>(new QueryDefinition(queryString));
            List<Person> results = new List<Person>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, Person item)
        {
            await this._container.UpsertItemAsync<Person>(item, new PartitionKey(id));
        }
    }
}
