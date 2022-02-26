using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace AzureApp31.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesAuthController : ControllerBase
    {
        static List<int> _values;

        static ValuesAuthController()
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

        //[Authorize(Roles = "Admin")]
        [HttpGet, ActionName("GetAll")]
        public ActionResult<List<int>> GetAll()
        {
            var currentUser = HttpContext.User;

            if (currentUser.HasClaim(c => c.Type == "Email"))
            {
                return _values;
            }

            return Forbid();
        }
    }
}
