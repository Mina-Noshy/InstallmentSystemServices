using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using MyWebAPI.Services.Account;
using MyWebAPI.Services.Helper;
using MyWebModels.Models.Account;
using MyWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterAsync(RegisterVM model)
        {
            var message = await _userService.RegisterAsync(model);

            if (message.State)
                return Ok(message);
            else
                return BadRequest(message);
        }

        [HttpPost]
        public async Task<ActionResult> RegisterAsAdminAsync(RegisterVM model)
        {
            if(!await _userService.AnyUser())
            {
                string _email = "mina-noshy@outlook.com";
                string _password = "666666";
                // here add user.
                var result = await _userService.CreateUserAsync(new AppUser
                {
                    FirstName = "Mina",
                    LastName = "Noshy",
                    UserName = _email,
                    Email = _email,
                    PhoneNumber = "01111257052",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                }, _password);

                if(!result)
                    return BadRequest(new ResponseVM { State = false, Title = "Oh baby", Message = "Well done baby, try playing again." });

                if (!await _userService.AnyRole())
                {
                    // here add all roles.
                    if(await _userService.CreateRoleAsync(RoleVM.Admin) 
                    && await _userService.CreateRoleAsync(RoleVM.Moderator)
                    && await _userService.CreateRoleAsync(RoleVM.User))
                    {
                        // here add user to role.
                        AppUser user = await _userService.GetByEmail(_email);
                        var roleMessage = await _userService.AddUserToRoleAsync(_email, RoleVM.Admin);
                        
                        if (roleMessage.State)
                            return Ok(roleMessage);
                        else
                            return BadRequest(roleMessage);
                    }

                }
                else
                    return BadRequest(new ResponseVM { State = false, Title = "Oh baby", Message = "Well done baby, try playing again." });
            }

            return BadRequest(new ResponseVM { State = false, Title = "Oh baby", Message = "Well done baby, try playing again." });

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UpdateUserInfoAsync(RegisterVM model)
        {
            var message = await _userService.UpdateUserInfoAsync(model);

            if (message.State)
                return Ok(message);
            else
                return BadRequest(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userName}/{role}")]
        public async Task<ActionResult> UpdateUserRoleAsync(string userName, string role)
        {
            AppUser user = await _userService.GetByName(userName);
            var message = await _userService.UpdateUserRoleAsync(user, role);

            if (message.State)
                return Ok(message);
            else
                return BadRequest(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userName}/{role}")]
        public async Task<ActionResult> RemoveUserFromRoleAsync(string userName, string role)
        {
            var message = await _userService.RemoveUserFromRoleAsync(userName, role);

            if (message.State)
                return Ok(message);
            else
                return BadRequest(message);
        }


        [HttpPost]
        public async Task<IActionResult> GetTokenAsync(TokenRequestVM model)
        {
            var result = await _userService.GetTokenAsync(model);

            if (result.IsAuthenticated)
            {
                SetRefreshTokenInCookie(result.RefreshToken);
                return Ok(result);
            }

            return Unauthorized(result);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{userName}/{role}")]
        public async Task<IActionResult> AddUserToRoleAsync(string userName, string role)
        {
            var message = await _userService.AddUserToRoleAsync(userName, role);

            if (message.State)
                return Ok(message);
            else
                return BadRequest(message);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _userService.RefreshTokenAsync(refreshToken);

            if (!string.IsNullOrEmpty(response.RefreshToken))
                SetRefreshTokenInCookie(response.RefreshToken);

            return Ok(response);
        }


        [HttpPost("{token}")]
        public async Task<IActionResult> RevokeToken(string token)
        {
            // accept token from request body or cookie
            var _userToken = token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(_userToken))
                return BadRequest(new ResponseVM { State = false, Title = "Error", Message = "Token is required" });

            var response = await _userService.RevokeToken(_userToken);

            if (!response)
                return NotFound(new ResponseVM { State = false, Title = "Error", Message = "Token not found" });

            return Ok(new ResponseVM { State = true, Title = "Success", Message = "Token revoked" });
        }

        [Authorize]
        [HttpGet("{userName}")]
        public async Task<IActionResult> GetRefreshTokens(string userName)
        {
            var user = await _userService.GetByName(userName);
            return Ok(user.RefreshTokens);
        }

        [HttpPost("{email}")]
        public async Task<ActionResult> IsEmailAvailable(string email)
        {
            if (await _userService.IsEmailAvailable(email))
                return Ok();

            return NotFound();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("{userId}/{days}")]
        public async Task<ActionResult> RenewSubscription(string userId, int days)
        {
            if (await _userService.RenewSubscription(userId, days))
                return Ok();

            return NotFound();
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> GetUserStoppedDate(string userId)
        {
            DateTime _date = await _userService.GetUserStoppedDate(userId);

            if (_date != default)
                return Ok(_date);

            return NotFound(_date);
        }

        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(30),
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    
    }
}
