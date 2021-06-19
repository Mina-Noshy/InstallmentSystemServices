using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        [HttpGet("{name}/{age}/{address}")]
        public ActionResult<string> MultiParams(string name, int age, string address)
        {
            return $"name is {name}, phone is {age}, address is {address}";
        }

    }
}
