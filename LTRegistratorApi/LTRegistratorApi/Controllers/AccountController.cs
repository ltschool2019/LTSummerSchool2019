using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LTTimeRegistrator.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LTRegistratorApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationContext context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

            //If there are no users, then add basic users.
            if (_userManager.Users.Count() == 0)
            {
                _ = RegisterUser(new IdentityUser { UserName = "Alice", Email = "alice@mail.ru" },  "aA123456!", "Administrator");
                _ = RegisterUser(new IdentityUser { UserName = "Bob", Email = "b0b@yandex.ru" }, "+B0o0B+", "Manager");
                _ = RegisterUser(new IdentityUser { UserName = "Eve", Email = "eve.99@yandex.ru" }, "1Adam!!!", "Employee");
                context.SaveChanges();
            }
        }

        /// <summary>
        /// The method attempts to register a user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        private async Task<bool> RegisterUser(IdentityUser user, string password, string role)
        {
            /* Passwords must be at least 6 characters.
             * Passwords must have at least one non alphanumeric character.
             * Passwords must have at least one digit ('0'-'9').
             * Passwords must have at least one lowercase ('a'-'z').
             * Passwords must have at least one uppercase ('A'-'Z').*/
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var addClaim = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));
                if (addClaim.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The method tries to authorize the user and return the JWT-token.
        /// </summary>
        /// <param name="model">user</param>
        /// <returns>JWT-token</returns>
        [HttpPost]
        public async Task<object> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == model.Email);
                return await GenerateJwtToken(appUser);
            }

            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
        }

        /// <summary>
        /// The method attempts to register a user and return the JWT-token.
        /// </summary>
        /// <param name="model">User</param>
        /// <returns>JWT-token</returns>
        [HttpPost]
        public async Task<object> Register([FromBody] RegisterDto model)
        {
            var user = new IdentityUser
            {
                UserName = model.Name,
                Email = model.Email,
            };

            if (await RegisterUser(user, model.Password, model.Role))
            {
                await _signInManager.SignInAsync(user, false);
            }

            throw new ApplicationException("ERROR_REGISTER");
        }

        /// <summary>
        /// Method generates JWT-token for user.
        /// </summary>
        /// <param name="user">The IdentityUser who has mail.</param>
        /// <returns>JWT-token</returns>
        private async Task<object> GenerateJwtToken(IdentityUser user)
        {
            var resultOfGetClaims = _userManager.GetClaimsAsync(user).Result;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };
            foreach (var claim in resultOfGetClaims)
            {
                claims.Add(claim);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}