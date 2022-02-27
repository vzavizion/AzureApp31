using AzureApp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureApp31.Services
{
    public interface ICosmosDbService
    {
        Task<IEnumerable<Person>> GetItemsAsync(string query);
        Task<Person> GetItemAsync(string id);
        Task AddItemAsync(Person item);
        Task UpdateItemAsync(string id, Person item);
        Task DeleteItemAsync(string id);
    }
}
