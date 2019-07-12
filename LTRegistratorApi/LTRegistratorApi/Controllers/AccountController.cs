using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LTRegistratorApi.Controllers
{
    /// <summary>
    /// The controller that is responsible for basic user operations.
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        /// <param name="userManager">Allows you to manage users</param>
        /// <param name="signInManager">Provides the APIs for user sign in</param>
        /// <param name="configuration">To use the file setting</param>
        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;

            //If there are no users, then add basic users.
            if (_userManager.Users.Count() == 0)
            {
                RegisterUser(new IdentityUser { UserName = "Alice", Email = "alice@mail.ru" },  "aA123456!", "Administrator");
                RegisterUser(new IdentityUser { UserName = "Bob", Email = "b0b@yandex.ru" }, "+B0o0B+", "Manager");
                RegisterUser(new IdentityUser { UserName = "Eve", Email = "eve.99@yandex.ru" }, "1Adam!!!", "Employee");
            }
        }

        /// <summary>
        /// The method attempts to register a user
        /// </summary>
        /// <param name="user">IdentityUser with UserName and Email</param>
        /// <param name="password">User password</param>
        /// <param name="role">User role</param>
        /// <returns>Did you register</returns>
        private bool RegisterUser(IdentityUser user, string password, string role)
        {
            /* Passwords must be at least 6 characters.
             * Passwords must have at least one non alphanumeric character.
             * Passwords must have at least one digit ('0'-'9').
             * Passwords must have at least one lowercase ('a'-'z').
             * Passwords must have at least one uppercase ('A'-'Z').*/
            var result = _userManager.CreateAsync(user, password).Result;
            var resultAddClaim = _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role)).Result;
            return result.Succeeded && resultAddClaim.Succeeded;
        }

        /// <summary>
        /// The method tries to authorize the user and return the JWT-token.
        /// </summary>
        /// <param name="model">LoginDto (user)</param>
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
                UserName = model.Email, //for PasswordSignInAsync
                Email = model.Email,
            };

            if (RegisterUser(user, model.Password, model.Role))
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
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //generate almost unique identifier for token
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                resultOfGetClaims.Single(claim => claim.Type == ClaimTypes.Role) //for now one
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); //signing algorithm
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"])); //how many days is the token valid

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