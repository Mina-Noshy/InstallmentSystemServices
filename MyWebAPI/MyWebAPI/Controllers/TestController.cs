using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebModels.Database;
using MyWebModels.Models.Account;
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
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly AppDbContext context;

        public TestController(UserManager<AppUser> userManager, 
                              RoleManager<IdentityRole> roleManager,
                              AppDbContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.context = context;
        }

        [HttpGet("{name}/{age}/{address}")]
        public ActionResult<string> param(string name, int age, string address)
        {
            return $"name is {name}, age is {age}, address is {address}";
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public ActionResult<string> admin()
        {
            return "admin role work successfully";
        }

        [Authorize]
        [HttpGet]
        public ActionResult<string> jwt()
        {
            return "jwt authorization work successfully";
        }

    }
}
