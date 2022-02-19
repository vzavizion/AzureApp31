using AzureApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AzureApp31.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        static List<int> _values;

        static ValuesController()
        {
            Init();
        }

        private static void Init()
        {
            _values = new List<int>();

            _values.Add(2);
            _values.Add(4);
            _values.Add(6);
        }

        [HttpGet, ActionName("GetAll")]
        public IEnumerable<int> GetAll()
        {
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
