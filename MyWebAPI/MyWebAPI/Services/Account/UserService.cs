using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyWebModels.Database;
using MyWebModels.Sittings;
using MyWebModels.Models.Account;
using MyWebModels.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyWebAPI.Services.Account
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(AppUser user, string password);
        Task<bool> CreateRoleAsync(string role);

        Task<ResponseVM> RegisterAsync(RegisterVM model);
        Task<ResponseVM> UpdateUserInfoAsync(UpdateUserInfoVM model);
        Task<ResponseVM> UpdateUserPasswordAsync(UpdateUserPasswordVM model);
        Task<ResponseVM> UpdateUserRoleAsync(AppUser user, string role);
        Task<ResponseVM> RemoveUserFromRoleAsync(string email, string role);
        Task<AuthenticationVM> GetTokenAsync(TokenRequestVM model);
        Task<ResponseVM> AddUserToRoleAsync(string email, string role);
        Task<AuthenticationVM> RefreshTokenAsync(string jwtToken);
        Task<bool> RevokeToken(string token);
        Task<bool> IsEmailAvailable(string email);
        Task<bool> IsUserNameAvailable(string userName);
        Task<AppUser> GetById(string id);
        Task<AppUser> GetByName(string userName);
        Task<AppUser> GetByEmail(string email);
        Task<bool> AnyUser();
        Task<bool> AnyRole();
        Task<bool> AnyUserRole();

        Task<ResponseVM> IncreaseSubscription(string userId, int days);
        Task<ResponseVM> DecreaseSubscription(string userId, int days);

        Task<DateTime> GetUserStoppedDate(string userId);

        Task<List<AppUserMobileVM>> GetAllUsersVM();
        Task<AuthenticationVM> GetUserDetailsVM(string userId);
        Task<List<string>> GetAllRolesVM();
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public UserService(UserManager<AppUser> userManager,
                            RoleManager<IdentityRole> roleManager,
                            IOptions<JWT> jwt, AppDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
        }

        public async Task<bool> CreateUserAsync(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                return true;

            return false;
        }
        public async Task<bool> CreateRoleAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                    return true;

                return false;
            }

            return false;
        }


        public async Task<ResponseVM> RegisterAsync(RegisterVM model)
        {
            var user = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);

            if(userWithSameEmail == null)
                userWithSameEmail = await _userManager.FindByNameAsync(model.Email);

            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return new ResponseVM { State = true, Title = "Success", Message = $"Success, User Registered with username {user.UserName}" };
                }
                else
                    return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when register email {user.Email }." };
            }
            else
            {
                return new ResponseVM { State = false, Title = "Error", Message = $"Error, Email {user.Email } is already registered." };
            }
        }

        public async Task<ResponseVM> UpdateUserInfoAsync(UpdateUserInfoVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                    return new ResponseVM { State = true, Title = "Success", Message = $"Success, User {user.UserName} has been updated successfully." };

                else
                    return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when update User {user.Email }." };
            }
            else
            {
                return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when update User {user.Email }." };
            }
        }

        public async Task<ResponseVM> UpdateUserRoleAsync(AppUser user, string role)
        {
            if (user != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role);
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, role);

                    if (result.Succeeded)
                        return new ResponseVM { State = true, Title = "Success", Message = $"Success, User {user.Email} has been added to role {role}." };

                    else
                        return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when add user {user.Email} to role {role}." };

                }
                else
                    return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when remove user {user.Email} from role." };
            }
            else
            {
                return new ResponseVM { State = false, Title = "Error", Message = $"Error, Email {user.Email } not found." };
            }
        }

        public async Task<ResponseVM> RemoveUserFromRoleAsync(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
                await _userManager.FindByNameAsync(email);

            if (user != null)
            {
                var result = await _userManager.RemoveFromRoleAsync(user, role);

                if (result.Succeeded)
                    return new ResponseVM { State = true, Title = "Success", Message = $"Success, User {user.Email} has been added to role {role}." };

                else
                    return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when remove user {user.Email} from role {role}." };
            }
            else
            {
                return new ResponseVM { State = false, Title = "Error", Message = $"Error, Email {user.Email } not found." };
            }
        }

        public async Task<AuthenticationVM> GetTokenAsync(TokenRequestVM model)
        {
            var authenticationModel = new AuthenticationVM();

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
                user = await _userManager.FindByEmailAsync(model.UserName);


            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Error, No accounts registered with {model.UserName}.";
                return authenticationModel;
            }
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);

                authenticationModel.UserId = user.Id;
                authenticationModel.IsAuthenticated = true;
                authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                authenticationModel.Email = user.Email;
                authenticationModel.UserName = user.UserName;
                authenticationModel.PhoneNumber = user.PhoneNumber;
                authenticationModel.FirstName = user.FirstName;
                authenticationModel.LastName = user.LastName;
                authenticationModel.StopAt = user.StopAt;
                var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
                authenticationModel.Roles = rolesList.ToList();


                if (user.RefreshTokens.Any(a => a.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.Where(a => a.IsActive == true).FirstOrDefault();

                    authenticationModel.RefreshToken = activeRefreshToken.Token;
                    authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
                }
                else
                {
                    var refreshToken = CreateRefreshToken();
                    authenticationModel.RefreshToken = refreshToken.Token;
                    authenticationModel.RefreshTokenExpiration = refreshToken.Expires;

                    user.RefreshTokens.Add(refreshToken);
                    _context.Update(user);
                    _context.SaveChanges();
                }

                return authenticationModel;
            }
            authenticationModel.IsAuthenticated = false;
            authenticationModel.Message = $"Error, Incorrect Credentials for user {user.Email}.";
            return authenticationModel;
        }

        private RefreshTokenVM CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);
                return new RefreshTokenVM
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow
                };

            }
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.Expire),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        public async Task<ResponseVM> AddUserToRoleAsync(string email, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                await _userManager.FindByNameAsync(email);

            if (!await _userManager.IsInRoleAsync(user, role))
            {
                await _userManager.AddToRoleAsync(user, role);
                return new ResponseVM { State = true, Title = "Success", Message = $"Success, Added {role} to user {user.Email}." };
            }
            return new ResponseVM { State = false, Title = "Error", Message = $"Error, Role {role} not found." };

        }

        public async Task<AuthenticationVM> RefreshTokenAsync(string token)
        {
            var authenticationModel = new AuthenticationVM();
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Error, Token did not match any users.";
                return authenticationModel;
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Error, Token Not Active.";
                return authenticationModel;
            }

            //Revoke Current Refresh Token
            refreshToken.Revoked = DateTime.UtcNow;

            //Generate new Refresh Token and save to Database
            var newRefreshToken = CreateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            //Generates new jwt
            authenticationModel.IsAuthenticated = true;
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);
            authenticationModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationModel.Roles = rolesList.ToList();
            authenticationModel.RefreshToken = newRefreshToken.Token;
            authenticationModel.RefreshTokenExpiration = newRefreshToken.Expires;
            return authenticationModel;
        }

        public async Task<bool> RevokeToken(string token)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            _context.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
                return false;

            return true;

        }

        public async Task<bool> IsUserNameAvailable(string userName)
        {
            if (await _userManager.FindByNameAsync(userName) != null)
                return false;

            return true;

        }

        public async Task<AppUser> GetById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<AppUser> GetByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<AppUser> GetByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> AnyUser()
        {
            return await _context.Users.AnyAsync();
        }

        public async Task<bool> AnyRole()
        {
            return await _context.Roles.AnyAsync();
        }

        public async Task<bool> AnyUserRole()
        {
            return await _context.UserRoles.AnyAsync();
        }

        public async Task<ResponseVM> IncreaseSubscription(string userId, int days)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseVM { State = false, Title = "Error", Message = "User not found!" };

            user.StopAt = user.StopAt.AddDays(days);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var userAfter = await _userManager.FindByIdAsync(userId);

                return new ResponseVM { State = true, Title = "Success", Message = userAfter.StopAt.Date.ToString() };
            }

            return new ResponseVM { State = false, Title = "Error", Message = "Something wrong!" };
        }

        public async Task<ResponseVM> DecreaseSubscription(string userId, int days)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new ResponseVM { State = false, Title = "Error", Message = "User not found!" };

            user.StopAt = user.StopAt.AddDays(days * -1);
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                var userAfter = await _userManager.FindByIdAsync(userId);

                return new ResponseVM { State = true, Title = "Success", Message = userAfter.StopAt.Date.ToString() };
            }

            return new ResponseVM { State = false, Title = "Error", Message = "Something wrong!" };
        }

        public async Task<DateTime> GetUserStoppedDate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                return user.StopAt;

            return default;
        }

        public async Task<ResponseVM> UpdateUserPasswordAsync(UpdateUserPasswordVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                    return new ResponseVM { State = true, Title = "Success", Message = $"Success, {user.UserName} password has been updated successfully." };

                else
                    return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when update user password for user {user.Email} ." };
            }
            else
            {
                return new ResponseVM { State = false, Title = "Error", Message = $"Error, Some error when update user password for user {user.Email }." };
            }
        }



        public async Task<List<AppUserMobileVM>> GetAllUsersVM()
        {
            return await _userManager.Users.OrderBy(x => x.FirstName)
                .Select(x => new AppUserMobileVM
            {
                UserId = x.Id,
                UserName = x.UserName,
                Email = x.Email,
                FullName = x.FirstName + x.LastName
            }).ToListAsync();
        }

        public async Task<AuthenticationVM> GetUserDetailsVM(string userId)
        {
            var authenticationModel = new AuthenticationVM();

            var user = await _userManager.FindByIdAsync(userId);


            if (user == null)
            {
                authenticationModel.IsAuthenticated = false;
                authenticationModel.Message = $"Error, No accounts registered with id {userId}.";
                return authenticationModel;
            }


            authenticationModel.UserId = user.Id;
            authenticationModel.IsAuthenticated = true;
            authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            authenticationModel.PhoneNumber = user.PhoneNumber;
            authenticationModel.FirstName = user.FirstName;
            authenticationModel.LastName = user.LastName;
            authenticationModel.StopAt = user.StopAt;
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            authenticationModel.Roles = rolesList.ToList();

            return authenticationModel;
        }

        public async Task<List<string>> GetAllRolesVM()
        {
            return await _roleManager.Roles.OrderBy(x => x.Name)
                .Select(x => x.Name).ToListAsync();
        }
    }
}
