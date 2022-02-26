using AzureApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nest;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureApp31.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        static List<int> _values;
        static List<ApplicationUser> _users;

        //private readonly IConnectionMultiplexer _redis;               
        //private readonly IDistributedCache _distributedCache;
        private readonly ILogger<ValuesController> _logger;
        //private readonly IElasticsearchHelper _elasticsearchHelper;

        static ValuesController()
        {
            Init();
        }
        public ValuesController(
            ILogger<ValuesController> logger
            //IConnectionMultiplexer redis,
            //IDistributedCache distributedCache,
            //IElasticsearchHelper elasticsearchHelper
            )
        {
            _logger = logger;
            //_redis = redis;
            //_distributedCache = distributedCache;
            //_elasticsearchHelper = elasticsearchHelper;

            //AddToCache();
        }

        private static void Init()
        {
            _values = new List<int>();

            _values.Add(2);
            _values.Add(4);
            _values.Add(6);

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
        }

        [HttpGet, ActionName("GetAll")]
        public IEnumerable<int> GetAll()
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

        [HttpGet("{id}"), ActionName("GetById")]
        public int GetById([FromRoute] int id)
        {
            return _values[id];
        }

        [HttpGet, ActionName("GetByIdQ")]
        public int GetByIdQ([FromQuery] int id)
        {
            return _values[id];
        }

        [HttpPost, ActionName("Add")]
        public IActionResult Add(Item item)
        {
            _values.Add(item.value);
            _values.Add(item.value + 2);
            _values.Add(item.value + 4);

            return Ok(_values);
        }

        [HttpPost, ActionName("Clear")]
        public IActionResult Clear()
        {
            Init();

            return Ok(_values);
        }
    }
}
