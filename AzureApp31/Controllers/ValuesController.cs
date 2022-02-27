using AzureApp;
using AzureApp31.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nest;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace AzureApp31.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        static List<Person> _values;
        static List<ApplicationUser> _users;

        IMongoCollection<Person> _persons;

        //private readonly IConnectionMultiplexer _redis;               
        //private readonly IDistributedCache _distributedCache;
        private readonly ILogger<ValuesController> _logger;
        //private readonly IElasticsearchHelper _elasticsearchHelper;
        //private readonly ICosmosDbService _cosmosDbService;

        static ValuesController()
        {
            Init();
        }
        public ValuesController(
            ILogger<ValuesController> logger
            //IConnectionMultiplexer redis,
            //IDistributedCache distributedCache,
            //IElasticsearchHelper elasticsearchHelper
            //ICosmosDbService cosmosDbService
            )
        {
            _logger = logger;
            //_redis = redis;
            //_distributedCache = distributedCache;
            //_elasticsearchHelper = elasticsearchHelper;
            //_cosmosDbService = cosmosDbService;

            AddToCache();
        }

        private static void Init()
        {
            //Memory
            _values = new List<Person>();

            _values.Add(new Person() { id = 1, name = "aaa" });
            _values.Add(new Person() { id = 2, name = "bbb" });
            _values.Add(new Person() { id = 3, name = "ccc" });

            //Elastic
            _users = new List<ApplicationUser>();
            for (int i=0; i<_values.Count; i++)
            {
                _users.Add(new ApplicationUser()
                {
                    LoginName = i.ToString()
                });
            }
        }

        private void AddToCache()
        {
            //REDIS
            //var redisValue = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_values));

            //var options = new DistributedCacheEntryOptions()
            //        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
            //        .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            //_distributedCache.Set("key1", redisValue, options);


            //var db = _redis.GetDatabase();
            //db.StringSet("key2", JsonConvert.SerializeObject(_values));


            //ELASTIC
            //_elasticsearchHelper.SaveMany(_users);
            //_elasticsearchHelper.SaveSingle(_users[0]);

            //Mongo
            //for (int i = 0; i < _values.Count; i++)
            //{
            //    _cosmosDbService.AddItemAsync(_values[i]);
            //}

            //Mongo Other
            ConfigureMongo();
            var isAdded = _persons.Find(p => p.id == 1).FirstOrDefault() != null;
            if (!isAdded)
            {
                for (int i = 0; i < _values.Count; i++)
                {
                    _persons.InsertOneAsync(_values[i]);
                }
            }
        }

        private void ConfigureMongo()
        {
            string connectionString = @"mongodb://796c9f5b-0ee0-4-231-b9ee:kYuPLrimL3461ePWq8TM7wClD3I6I001baG0obwvMgWNxOwKxRirooWm0yUQmVytSZCgB6gdm08NCdM03u2eXg==@796c9f5b-0ee0-4-231-b9ee.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@796c9f5b-0ee0-4-231-b9ee@";
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);
            var db = mongoClient.GetDatabase("vzazdb");
            _persons = db.GetCollection<Person>("Person");

            //var a = db.ListCollectionNames();
            //var a1 = a.MoveNext();            
        }

        [HttpGet, ActionName("GetAll")]
        public IEnumerable<Person> GetAll()
        {
            //Exception to Elastic
            //_logger.LogWarning(String.Format("GetAll Request {0}", DateTime.Now));

            //try
            //{
            //    throw new Exception("GetAll Exception");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, "Unknown error GetAll Exception");
            //}


            //REDIS
            //List<int> values = new List<int>();

            //var redisValue = _distributedCache.Get("key1");
            //if (redisValue != null)
            //{
            //    values = JsonConvert.DeserializeObject<List<int>>(Encoding.UTF8.GetString(redisValue));
            //}

            //var db = _redis.GetDatabase();
            //var key2 = db.StringGet("key2");

            //return values;


            return _values;
        }

        [HttpGet, ActionName("GetAllMongo")]
        public IEnumerable<Person> GetAllMongo()
        {
            return _persons.AsQueryable();
            //return _persons.Find(p => true).ToList();
        }

        [HttpGet("{id}"), ActionName("GetById")]
        public Person GetById([FromRoute] int id)
        {
            return _values.FirstOrDefault(p => p.id == id);
        }

        [HttpGet("{id}"), ActionName("GetByIdMongo")]
        public Person GetByIdMongo([FromRoute] int id)
        {
            return _persons.Find(p => p.id == id).FirstOrDefault();
        }

        [HttpGet, ActionName("GetByIdQ")]
        public Person GetByIdQ([FromQuery] int id)
        {
            return _values.FirstOrDefault(p => p.id == id);
        }

        [HttpPost, ActionName("Add")]
        public IActionResult Add(Person person)
        {
            _values.Add(person);
            
            return Ok(_values);
        }

        [HttpPost, ActionName("AddMongo")]
        public async Task<IActionResult> AddMongo(Person person)
        {
            await _persons.InsertOneAsync(person);

            return Ok(_persons.AsQueryable());
        }

        [HttpPost, ActionName("Clear")]
        public IActionResult Clear()
        {
            Init();

            return Ok(_values);
        }

        [HttpPost, ActionName("DeleteMongo")]
        public async Task<IActionResult> DeleteMongo(Person person)
        {
            await _persons.DeleteOneAsync<Person>(p => p.id == person.id);

            return Ok(_persons.AsQueryable());
        }
    }
}
